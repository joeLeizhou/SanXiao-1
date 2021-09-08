using Orcas.AssetBuilder;
using Orcas.Core;
using Orcas.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Orcas.Resources
{
    public enum PackHotfixState
    {
        None,
        NeedDownload,
        DownloadOver,
        Error,
    }

    /// <summary>
    /// 分包热更管理器
    /// </summary>
    public class PackHotfixManager : SingletonBase<PackHotfixManager>
    {
        private Dictionary<string, CsvFileListInfo> _packsFilstList, _fileListDownloading;
        private PackAssetBundleManifest _packsManifest;

        private Dictionary<int, Dictionary<string, CsvFileListInfo>> _packFileListDic;
        private Dictionary<int, Coroutine> _packCoDic;
        private Action<PackHotfixState> _onCheckFinish, _onDownloadFinish;

        private int _needDownAllCount = 1, _downloadedCount = 0;
        public float DownloadProgers { get => (float)_downloadedCount / _needDownAllCount; }
        public bool IsChecking { get; private set; } = false;
        public int DownloadingPack { get; private set; } = -1;
        public bool IsSlowDownload { get; private set; } = false;

        /// <summary>
        /// 拉取所有pack的filelist
        /// </summary>
        /// <param name="packs"></param>
        /// <param name="callBack"></param>
        public void PullAllFileList(List<int> packs, Action callBack)
        {
            var sceneFileListPath = PathConst.DestPath + PathConst.PacksFileListName;
            if (File.Exists(sceneFileListPath))
                _packsFilstList = CsvFileListUtil.LoadDicAtPath(sceneFileListPath);
            else
                _packsFilstList = new Dictionary<string, CsvFileListInfo>();

            var sceneManifestPath = PathConst.DestPath + PathConst.PacksManifestName;
            if (File.Exists(sceneManifestPath))
                _packsManifest = JsonUtility.FromJson<PackAssetBundleManifest>(File.ReadAllText(sceneManifestPath));
            else
                _packsManifest = new PackAssetBundleManifest();
            _packsManifest.Init();

            _packFileListDic = new Dictionary<int, Dictionary<string, CsvFileListInfo>>();
            _packCoDic = new Dictionary<int, Coroutine>();
            int downloadCount = 0;
            int stageCount = packs.Count;
            for (int i = 0; i < stageCount; i++)
            {
                int pack = packs[i];
                _packCoDic[pack] = DownloadPackFileList(pack, (error) =>
                {
                    downloadCount++;
                    _packCoDic.Remove(pack);
                    if (downloadCount == stageCount) callBack?.Invoke();
                });
            }
        }
        /// <summary>
        /// 下载一个pack的filelist
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private Coroutine DownloadPackFileList(int pack, Action<string> action)
        {
            var path = PathConst.GetPackOSPath(PathConst.GetServerRootUrl()) + PathConst.GetPackFileList(pack);
            Debug.Log("download " + path);
            return CoroutineManager.Instance.StartCoroutine(DownloadManager.Instance.DownloadOne(path, (handler, error) =>
            {
                if (handler != null)
                {
                    var fileLists = CsvFileListUtil.LoadDicByContent(handler.text);
                    _packFileListDic.Add(pack, fileLists);
                }
                action?.Invoke(error);
            }));
        }
        /// <summary>
        /// 检查pack是否需要更新
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="action"></param>
        /// <param name="forceReload">是否重新下载配置</param>
        public void CheckPack(int pack, Action<PackHotfixState> action, bool forceReload = false)
        {
            IsChecking = true;
            _onCheckFinish = action;
            if (_packFileListDic.ContainsKey(pack) && forceReload == false)
            {
                var needDownload = CheckNeedDownload(pack);
                action?.Invoke(needDownload ? PackHotfixState.NeedDownload : PackHotfixState.None);
                IsChecking = false;
            }
            else
            {
                if (_packCoDic.TryGetValue(pack, out var value))
                {
                    CoroutineManager.Instance.StopCoroutine(value);
                }

                _packCoDic[pack] = DownloadPackFileList(pack, (error) =>
                 {
                     _packCoDic.Remove(pack);
                     if (string.IsNullOrEmpty(error))
                     {
                         var needDownload = CheckNeedDownload(pack);
                         action?.Invoke(needDownload ? PackHotfixState.NeedDownload : PackHotfixState.None);
                     }
                     else
                     {
                         action?.Invoke(PackHotfixState.Error);
                     }
                     IsChecking = false;
                 });
            }
        }
        /// <summary>
        /// 检查md5，判断是否pack是否有文件需要更新
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        private bool CheckNeedDownload(int pack)
        {
            var stageFileList = _packFileListDic[pack];
            foreach (var item in stageFileList)
            {
                if (_packsFilstList.TryGetValue(item.Key, out var value))
                {
                    if (value.BundleName != item.Value.BundleName)
                        return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 对比md5，获取下载队列
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        private Queue<DownloadPair> GetDownloadQueue(int pack)
        {
            var queue = new Queue<DownloadPair>();
            _fileListDownloading = _packFileListDic[pack];
            foreach (var item in _fileListDownloading)
            {
                if (_packsFilstList.TryGetValue(item.Key, out var value))
                {
                    if (value.BundleName != item.Value.BundleName)
                        queue.Enqueue(new DownloadPair(item.Value.ID, item.Value.BundleName));
                }
                else
                {
                    queue.Enqueue(new DownloadPair(item.Value.ID, item.Value.BundleName));
                }
            }
            return queue;
        }
        private float _startDownloadTime = 0f;
        /// <summary>
        /// 开始更新一个stage，同时只能有一个stage在下载，DownloadingStage==-1表示没有下载
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="action"></param>
        /// <param name="isSlow"></param>
        public void StartDownload(int pack, Action<PackHotfixState> action, bool isSlow = false)
        {
            DownloadingPack = pack;
            IsSlowDownload = isSlow;
            _onDownloadFinish = action;
            _startDownloadTime = Time.realtimeSinceStartup;
            var _needDownQueue = GetDownloadQueue(pack);
            _needDownAllCount = _needDownQueue.Count;
            _downloadedCount = 0;
            var srcRootPath = PathConst.GetPackOSPath(PathConst.GetServerRootUrl()) + PathConst.AssetBundlePath;
            DownloadManager.Instance.SetRootPath(srcRootPath, PathConst.DestPath);
            DownloadManager.Instance.StartDownload(_needDownQueue, OnDownloadOne, OnDownloadAll, isSlow ? 3 : 6);
        }

        private void OnDownloadOne(DownloadPair downloadPair, string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                var fileInfo = _fileListDownloading[downloadPair.Key];
                _packsFilstList[downloadPair.Key] = fileInfo;
                if (fileInfo.ID.StartsWith(PathConst.GetCustomPackManifest(0), StringComparison.Ordinal))
                    CombinePacksManifest(fileInfo.BundleName);

                Debug.Log("download " + downloadPair.Key);
            }
            else
            {
                Debug.LogError("download " + downloadPair.Key + ",e:" + error);
            }
            _downloadedCount++;
        }

        private void CombinePacksManifest(string customManifestName)
        {
            var customManifest = PackAssetBundleManifest.Load(PathConst.DestPath + customManifestName);
            if (customManifest != null)
            {
                customManifest.Init();
                foreach (var item in customManifest.Infos)
                {
                    _packsManifest.Infos[item.Key] = item.Value;
                }
            }
        }

        private void OnDownloadAll()
        {
            var sceneFileListPath = PathConst.DestPath + PathConst.PacksFileListName;
            CsvFileListUtil.Save(_packsFilstList, sceneFileListPath);
            var sceneManifestPath = PathConst.DestPath + PathConst.PacksManifestName;
            _packsManifest.Save(sceneManifestPath, true);
            DownloadingPack = -1;
            _downloadedCount = _needDownAllCount;
            ResourceLoaderManager.AssetLoader.LoadFileLists();
            _onDownloadFinish?.Invoke(PackHotfixState.DownloadOver);
            Debug.Log("download time " + (Time.realtimeSinceStartup - _startDownloadTime).ToString());
        }
        /// <summary>
        /// 结束下载
        /// </summary>
        public void StopDownload()
        {
            if (DownloadingPack != -1)
            {
                DownloadManager.Instance.StopAllDownload();
                var sceneFileListPath = PathConst.DestPath + PathConst.PacksFileListName;
                CsvFileListUtil.Save(_packsFilstList, sceneFileListPath);
                DownloadingPack = -1;
            }
        }
    }
}