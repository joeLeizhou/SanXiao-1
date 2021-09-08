using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Core;
using Orcas.Core.Tools;
using Orcas.Csv;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Orcas.AssetBuilder;

namespace Orcas.Resources
{
    public enum HotfixState
    {
        Install = 0,
        InstallOver,
        ForceUpdate,
        NeedDown,
        DownOver,
    }
    public partial class HotFixManager : SingletonBase<HotFixManager>
    {

        private const string HotVersionKey = "hot_version";
        private const string buildGUIDKey = "build_guid";
        private HotfixVersion _loadingVersion;
        private int _needDownAllCount = 1, _downloadedCount = 0;

        public bool IsHotfix { get; private set; }
        public float DownloadProgerss { get => (float)_downloadedCount / _needDownAllCount; }

        /// <summary>
        /// 检查热更
        /// </summary>
        /// <param name="callback"></param>
        public void CheckHotfix(Action<HotfixState> callback)
        {
            _onHotfixState = callback;
            IsHotfix = true;
            CoroutineManager.Instance.StartCoroutine(CoDownloadVersion());
        }
        private Dictionary<string, CsvHotfix> LoadCsvHotfix(string content)
        {
            var dic = CsvLoader<string>.Import<CsvHotfix>(content, false);
            var result = new Dictionary<string, CsvHotfix>(dic.Count);
            foreach (var item in dic)
            {
                result.Add(item.Key, item.Value as CsvHotfix);
            }
            return result;
        }
        float _startHotfixTime;
        Dictionary<string, CsvHotfix> _versionLists;
        Dictionary<string, CsvFileListInfo> _fileListMain, _fileListDownloading;
        /// <summary>
        /// 下载热更配置文件，判断是否要强更
        /// </summary>
        /// <returns></returns>
        private IEnumerator CoDownloadVersion()
        {
            _versionLists = null;
            _startHotfixTime = Time.realtimeSinceStartup;
            var errorMsg = "";
            yield return Download(PathConst.GetHotVersionPath("", PathConst.AppVer), (handler, error) =>
            {
                errorMsg = error;
                if (handler != null) _versionLists = LoadCsvHotfix(handler.text);
            }, PathConst.ServerUrlArr.Length);

            if (_versionLists == null)
            {
                OnError?.Invoke(HotfixErrorCode.LoadVersionFail, errorMsg);
                IsHotfix = false;
                yield break;
            }
            yield return CheckNeedHotFix();
        }
        /// <summary>
        /// 检查是否需要热更
        /// <remarks> 第一次或者下载完成一次热更自后调用 </remarks> 
        /// </summary>
        private IEnumerator CheckNeedHotFix()
        {
            yield return new WaitForSeconds(0.1f);
            var savedVer = new HotfixVersion(PlayerPrefs.GetString(HotVersionKey, Application.version));
            var savedVerStr = savedVer.ToString();
            Debug.Log("check hotfix " + savedVerStr);
            if (_versionLists.ContainsKey(savedVerStr) == false)
            {
                Debug.LogError(savedVerStr + " dose not exist hotfix");
                _onHotfixState?.Invoke(HotfixState.DownOver);
                IsHotfix = false;
                yield break;
            }
            var hotfix = _versionLists[savedVerStr];
            var hotVer = new HotfixVersion(hotfix.HotVer);
            if (hotfix.ForceUpdate)
            {
                _onHotfixState?.Invoke(HotfixState.ForceUpdate);
                IsHotfix = false;
            }
            else if (hotVer > savedVer)
            {
                _downloadedCount = 0;
                _needDownAllCount = 1;
                _onHotfixState?.Invoke(HotfixState.NeedDown);
                _loadingVersion = hotVer;
                CoroutineManager.Instance.StartCoroutine(CoDownHotfix());
            }
            else
            {
                _onHotfixState?.Invoke(HotfixState.DownOver);
                IsHotfix = false;
                Debug.Log("hotfix time " + (Time.realtimeSinceStartup - _startHotfixTime).ToString());
            }
        }
        /// <summary>
        /// 下载一次热更下载列表，并且开始下载文件
        /// </summary>
        /// <returns></returns>
        private IEnumerator CoDownHotfix()
        {
            _fileListDownloading = null;
            var errorMsg = "";
            var filelistPath = PathConst.GetHotfixOSVersionPath("", _loadingVersion, PathConst.AppVer) + PathConst.FileListName;
            yield return Download(filelistPath, (handler, error) =>
            {
                errorMsg = error;
                if (handler != null) _fileListDownloading = CsvFileListUtil.LoadDicByContent(handler.text);
            }, PathConst.ServerUrlArr.Length);

            if (_fileListDownloading == null)
            {
                OnError?.Invoke(HotfixErrorCode.LoadFileListFail, errorMsg);
                IsHotfix = false;
                yield break;
            }
            _fileListMain = CsvFileListUtil.LoadDicAtPath(PathConst.DestPath + PathConst.FileListName);
            var queue = GetDownloadingQueue();
            Debug.Log("queue count " + queue.Count);
            _downloadedCount = 0;
            _needDownAllCount = queue.Count + 1;

            var srcRootPath = PathConst.GetHotfixOSBundlePath(PathConst.GetServerRootUrl(), PathConst.AppVer);
            DownloadManager.Instance.SetRootPath(srcRootPath, PathConst.DestPath);
            DownloadManager.Instance.StartDownload(queue, OnHotfixOne, OnHotfixAll);
        }
        /// <summary>
        /// 对比本地文件md5，找出需要更新的文件
        /// </summary>
        /// <returns></returns>
        private Queue<DownloadPair> GetDownloadingQueue()
        {
            var queue = new Queue<DownloadPair>(_fileListDownloading.Count);
            foreach (var item in _fileListDownloading)
            {
                if (_fileListMain.TryGetValue(item.Key, out var value))
                {
                    if (value.BundleName != item.Value.BundleName)
                        queue.Enqueue(new DownloadPair(item.Key, item.Value.BundleName));
                }
                else
                {
                    queue.Enqueue(new DownloadPair(item.Key, item.Value.BundleName));
                }
            }
            return queue;
        }

        /// <summary>
        /// 单个热更文件下载完成
        /// </summary>
        /// <param name="downloadPair"></param>
        /// <param name="error"></param>
        private void OnHotfixOne(DownloadPair downloadPair, string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                _fileListMain[downloadPair.Key] = _fileListDownloading[downloadPair.Key];
                _downloadedCount++;
                Debug.Log("downloaded " + downloadPair.Key);
            }
            else
            {
                OnError?.Invoke(HotfixErrorCode.DownloadFail, downloadPair.Key + ":" + error);
                Debug.LogError("downloaded " + downloadPair.Key + ",e:" + error);
            }
        }
        /// <summary>
        /// 一次热更列表全部下载完成
        /// </summary>
        private void OnHotfixAll()
        {
            if (_loadingVersion.Equals(new HotfixVersion("0.0.0")))
            {
                Debug.LogError("download all " + _loadingVersion);
                return;
            }

            CsvFileListUtil.Save(_fileListMain, PathConst.DestPath + PathConst.FileListName);
            Debug.Log("download all " + _loadingVersion.ToString());
            PlayerPrefs.SetString(HotVersionKey, _loadingVersion.ToString());
            PlayerPrefs.Save();

            CoroutineManager.Instance.StartCoroutine(CheckNeedHotFix());
        }

        public void OnPause()
        {
            if (IsInstalling)
                CsvFileListUtil.Save(_fileListMain, PathConst.DestPath + PathConst.FileListName);
        }

        public void OnUpdate()
        {
            // 安装结束需要在主线程中执行
            if (IsInstalling && IsInstallFinish)
                OnInstallOver();
        }

        /// <summary>
        /// 下载一个文件，失败之后切换服务器重试
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        /// <param name="leftTryTimes"></param>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        private IEnumerator Download(string path, Action<DownloadHandler, string> action, int leftTryTimes = 3, float delayTime = 1f)
        {
            string url = PathConst.GetServerRootUrl() + path;
            Debug.Log("download " + url);
            string errorMsg = "";
            yield return DownloadManager.Instance.DownloadOne(url, (handler, error) =>
            {
                errorMsg = error;
                if (handler != null)
                {
                    action?.Invoke(handler, error);
                }
            });
            if (string.IsNullOrEmpty(errorMsg) == false)
            {
                leftTryTimes--;
                if (leftTryTimes <= 0)
                {
                    action?.Invoke(null, errorMsg);
                }
                else
                {
                    PathConst.GetServerRootUrl(1);
                    yield return new WaitForSeconds(delayTime);
                    yield return Download(path, action, leftTryTimes, delayTime);
                }
            }
        }
    }
}