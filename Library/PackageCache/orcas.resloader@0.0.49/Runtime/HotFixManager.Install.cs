using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Core;
using Orcas.Core.Tools;
using System;
using System.IO;
using System.Threading.Tasks;
using Orcas.AssetBuilder;

namespace Orcas.Resources
{
    public enum HotfixErrorCode
    {
        None = 0,
        CopyFileFail,
        SaveFileFail,
        LoadVersionFail,
        LoadFileListFail,
        DownloadFail,
    }
    public partial class HotFixManager : SingletonBase<HotFixManager>
    {
        public event Action<HotfixErrorCode, string> OnError;
        private Action<HotfixState> _onInstallState, _onHotfixState;
        private int _copyFileAllCount = 1;
        private int _copyedFileCount = 0;
        public bool IsInstalling { get; private set; } = false;
        public bool IsInstallFinish { get; private set; } = true;
        public float InstallProgress { get => (float)_copyedFileCount / _copyFileAllCount; }
        /// <summary>
        /// 检查是否初次安装，或者关键文件丢失。
        /// 需要在检查热更之前调用。
        /// <remarks> 
        /// !!! 安装完成之后，本地记录热更版本号比安装包版本号大0.0.1 
        /// !!! OnUpdate 和 OnPause 函数需要外部手动调用
        /// </remarks>
        /// </summary>
        /// <param name="callback"></param>
        public void CheckInstall(Action<HotfixState> callback)
        {
            _onInstallState = callback;
            var isNewVersion = PlayerPrefs.GetString(buildGUIDKey, "") != Application.buildGUID;
            var isFileListMissing = File.Exists(PathConst.DestPath + PathConst.FileListName) == false;
            if (isNewVersion || isFileListMissing)
            {
                callback?.Invoke(HotfixState.Install);
                IsInstalling = true;
                IsInstallFinish = false;
                _startCopyTime = Time.realtimeSinceStartup;
#if UNITY_ANDROID && !UNITY_EDITOR
                CoroutineManager.Instance.StartCoroutine(CoMoveStreamingAsset());
#else
                Task.Run(MoveStreamingAsset);
#endif
            }
            else
            {
                IsInstalling = false;
                callback?.Invoke(HotfixState.InstallOver);
            }
        }
        private string _installError;
        private List<CsvFileListInfo> _copyFileList = null;

        List<string> _packsFileListNames = new List<string>();
        List<string> _packsManifestNames = new List<string>();
        float _startCopyTime = 0;
        /// <summary>
        /// 拷贝文件到可读写目录
        /// </summary>
        private void MoveStreamingAsset()
        {
            OnInstallStart();
            _copyFileList = CsvFileListUtil.LoadAtPath(PathConst.SourceOSPath + PathConst.CopyFileListName);
            _copyFileAllCount = _copyFileList.Count + 1;
            _copyedFileCount = 0;
            _packsFileListNames.Clear();
            _packsManifestNames.Clear();
            Debug.Log("need copy " + _copyFileAllCount + " to " + PathConst.DestPath);
            foreach (var item in _copyFileList)
            {
                string error = "";
                try
                {
                    File.Copy(PathConst.SourceOSPath + item.BundleName, PathConst.DestPath + item.BundleName, true);
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
                OnCopyOne(new DownloadPair(item.ID, item.BundleName), error);
            }
            OnCopyAll();
        }

        /// <summary>
        /// 通过下载的方式，拷贝文件到可读写目录。
        /// <remark> android的streamingAssetsPath 不能直接当做文件读取，需要走下载流程</remark>
        /// </summary>
        /// <returns></returns>
        private IEnumerator CoMoveStreamingAsset()
        {
            OnInstallStart();
            yield return DownloadManager.Instance.DownloadOne(PathConst.SourceOSUrl + PathConst.CopyFileListName, (handler, error) =>
            {
                _copyFileList = CsvFileListUtil.LoadByContent(handler.text);
            });
            _copyFileAllCount = _copyFileList.Count + 1;
            _copyedFileCount = 0;
            _packsFileListNames.Clear();
            _packsManifestNames.Clear();
            Debug.Log("need copy " + _copyFileAllCount);
            var queue = new Queue<DownloadPair>(_copyFileList.Count);
            foreach (var item in _copyFileList)
            {
                queue.Enqueue(new DownloadPair(item.ID, item.BundleName));
            }
            DownloadManager.Instance.SetRootPath(PathConst.SourceOSUrl, PathConst.DestPath);
            DownloadManager.Instance.StartDownload(queue, OnCopyOne, OnCopyAll);
        }
        /// <summary>
        /// 拷贝一个文件完成
        /// </summary>
        /// <param name="downloadPair"></param>
        /// <param name="error"></param>
        private void OnCopyOne(DownloadPair downloadPair, string error)
        {
            _installError = error;
            if (string.IsNullOrEmpty(error))
            {
                var fileName = downloadPair.Key;
                if (fileName.StartsWith(PathConst.FileListShortName, StringComparison.Ordinal) && fileName != PathConst.FileListName)
                    _packsFileListNames.Add(downloadPair.Value);
                else if (fileName.StartsWith(PathConst.GetCustomPackManifest(0), StringComparison.Ordinal))
                    _packsManifestNames.Add(downloadPair.Value);
                _copyedFileCount++;
                //Debug.Log("copyed file " + downloadPair.Value);
            }
            else
            {
                Debug.LogError("copyed file " + downloadPair.Key + ":" + downloadPair.Value + ",e:" + error);
            }
        }
        /// <summary>
        /// 拷贝所有文件完成
        /// </summary>
        private void OnCopyAll()
        {
            MergePacksFileList();
            _copyedFileCount = _copyFileAllCount;
            IsInstallFinish = true;
        }

        private void OnInstallStart()
        {
            if (Directory.Exists(PathConst.DestPath) == false)
                Directory.CreateDirectory(PathConst.DestPath);
            _copyFileList = new List<CsvFileListInfo>();
            _installError = "";
        }
        /// <summary>
        /// 安装结束
        /// </summary>
        private void OnInstallOver()
        {
            if (string.IsNullOrEmpty(_installError) == false)
                OnError?.Invoke(HotfixErrorCode.SaveFileFail, _installError);
            Debug.Log("install time " + (Time.realtimeSinceStartup - _startCopyTime));
            IsInstalling = false;
            IsInstallFinish = false;
            _onInstallState?.Invoke(HotfixState.InstallOver);
            PlayerPrefs.SetString(HotVersionKey, Application.version);
            PlayerPrefs.SetString(buildGUIDKey, Application.buildGUID);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// 合并场景文件的filelist到同一个文件，方便使用
        /// </summary>
        private void MergePacksFileList()
        {
            if (_packsFileListNames != null && _packsFileListNames.Count > 0)
            {
                var packFileListPath = PathConst.DestPath + PathConst.PacksFileListName;
                var packsFilstList = new Dictionary<string, CsvFileListInfo>();
                if (File.Exists(packFileListPath))
                {
                    packsFilstList = CsvFileListUtil.LoadDicAtPath(packFileListPath);
                }

                for (int i = 0; i < _packsFileListNames.Count; i++)
                {
                    var stagefileList = CsvFileListUtil.LoadDicAtPath(PathConst.DestPath + _packsFileListNames[i]);
                    foreach (var item in stagefileList)
                    {
                        packsFilstList[item.Key] = item.Value;
                    }
                }
                CsvFileListUtil.Save(packsFilstList, packFileListPath);
            }
            if (_packsManifestNames != null && _packsManifestNames.Count > 0)
            {
                var packsManifestPath = PathConst.DestPath + PathConst.PacksManifestName;
                PackAssetBundleManifest packsManifests;
                if (File.Exists(packsManifestPath))
                    packsManifests = JsonUtility.FromJson<PackAssetBundleManifest>(File.ReadAllText(packsManifestPath));
                else
                    packsManifests = new PackAssetBundleManifest();
                packsManifests.Init();

                for (int i = 0; i < _packsManifestNames.Count; i++)
                {
                    var customManifest = PackAssetBundleManifest.Load(PathConst.DestPath + _packsManifestNames[i]);
                    if (customManifest != null)
                    {
                        customManifest.Init();
                        //Debug.Log("init0 pack manifest " + customManifest.Infos.Count);
                        foreach (var item in customManifest.Infos)
                        {
                            packsManifests.Infos[item.Key] = item.Value;
                        }
                    }
                }
                packsManifests.Save(packsManifestPath, true);
                Debug.Log("save packs manifest " + packsManifests.Infos.Count);
            }
        }
        /// <summary>
        /// 清理所有缓存，清理完成之后需要重启app，然后走第一次安装的流程
        /// </summary>
        public void ClearAll()
        {
            if (Directory.Exists(PathConst.DestPath))
            {
                Directory.Delete(PathConst.DestPath, true);
            }
            PlayerPrefs.DeleteKey(buildGUIDKey);
        }
    }
}