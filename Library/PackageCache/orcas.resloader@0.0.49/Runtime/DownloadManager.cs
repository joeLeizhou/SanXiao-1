using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Core;
using UnityEngine.Networking;
using System;
using System.IO;

namespace Orcas.Resources
{
    public struct DownloadPair
    {
        public string Key;
        public string Value;
        public DownloadPair(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"k:{Key},v:{Value}";
        }
    }
    /// <summary>
    /// 下载管理器，开启多个协程同时下载并保存文件
    /// </summary>
    public class DownloadManager : MonoSingleton<DownloadManager>
    {
        public int MaxDowningCount { get; private set; }

        private const int KMaxDowningCount = 32;
        private string _srcRootPath;
        private string _destRootPath;
        private Queue<DownloadPair> _needDownloadQueue;
        private bool _isDownloading = false;
        private Action<DownloadPair, string> _onDownloadOne;
        private Action _onDownloadAll;
        private Coroutine[] _downloadingCos;
        private int _downloadingPipleMask, _downloadingPipleMaskInit;
        /// <summary>
        /// 设置下载源目录和保存目录
        /// </summary>
        /// <param name="srcRoot"></param>
        /// <param name="destRoot"></param>
        public void SetRootPath(string srcRoot, string destRoot)
        {
            _srcRootPath = srcRoot;
            _destRootPath = destRoot;
        }
        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="queue">下载队列</param>
        /// <param name="onOne">单个下载完成回调</param>
        /// <param name="onAll">全部下载完成回调</param>
        /// <param name="maxDowningCount">最大同时下载个数，最大值16 </param>
        public void StartDownload(Queue<DownloadPair> queue, Action<DownloadPair, string> onOne, Action onAll, int maxDowningCount = 8)
        {
            _needDownloadQueue = queue;
            _onDownloadOne = onOne;
            _onDownloadAll = onAll;
            _isDownloading = true;
            MaxDowningCount = maxDowningCount > KMaxDowningCount ? KMaxDowningCount : maxDowningCount;
            _downloadingCos = new Coroutine[MaxDowningCount];
            _downloadingPipleMaskInit = _downloadingPipleMask = 1 << MaxDowningCount - 1;
        }
        /// <summary>
        /// 结束所有下载
        /// </summary>
        public void StopAllDownload()
        {
            if (_downloadingCos != null)
            {
                for (int i = 0; i < _downloadingCos.Length; i++)
                {
                    _downloadingCos[i] = null;
                }
            }
            StopAllCoroutines();
            _isDownloading = false;
            _needDownloadQueue = null;
        }

        /// <summary>
        /// 检查是否有下载空闲位置，并且占位
        /// </summary>
        /// <returns></returns>
        private int FindEmptyIndex()
        {
            if (_downloadingPipleMask == 0)
                return -1;
            var index = 0;
            while ((_downloadingPipleMask & (1 << index)) == 0)
                index++;
            _downloadingPipleMask -= (1 << index);
            return index;
        }
        /// <summary>
        /// 还原占位
        /// </summary>
        /// <param name="index"></param>
        private void ReleaseIndex(int index)
        {
            _downloadingPipleMask += (1 << index);
            _downloadingCos[index] = null;
        }
        /// <summary>
        /// 检查是否全部空闲
        /// </summary>
        /// <returns></returns>
        private bool CheckEmpty()
        {
            return _downloadingPipleMask == _downloadingPipleMaskInit;
        }

        void Update()
        {
            if (_isDownloading)
            {
                if (_needDownloadQueue.Count > 0)
                {
                    var index = FindEmptyIndex();
                    if (index == -1)
                        return;
                    // 下载队列不为空，并且有空闲下载位置，开启新的下载
                    var downloadInfo = _needDownloadQueue.Dequeue();
                    _downloadingCos[index] = StartCoroutine(DownloadOne(_srcRootPath + downloadInfo.Value, (handler, error) =>
                    {
                        ReleaseIndex(index);
                        if (handler != null)
                            error = SaveData(downloadInfo.Value, handler.data);
                        _onDownloadOne?.Invoke(downloadInfo, error);
                    }));
                }
                else
                {
                    // 下载队列为空，并且全部下载位置空闲，表示下载完成
                    if (CheckEmpty())
                    {
                        _isDownloading = false;
                        _onDownloadAll?.Invoke();
                    }
                }
            }
        }
        /// <summary>
        /// 从网络下载文件，下载出错会重试
        /// </summary>
        /// <param name="name">文件名 (内部会拼接完整地址)</param>
        /// <param name="action">下载完成回调</param>
        /// <param name="canTryTimes">出错重试次数</param>
        /// <param name="delayTryTime">出错重试间隔</param>
        /// <returns></returns>
        public IEnumerator DownloadOne(string path, Action<DownloadHandler, string> action, int canTryTimes = 3, float delayTryTime = 1f)
        {
            string url = path + "?v=" + UnityEngine.Random.value;
            var request = UnityWebRequest.Get(url);
            request.timeout = 30;
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                var error = request.error;
                request.Dispose();
                canTryTimes--;
                if (canTryTimes <= 0)
                {
                    Debug.LogError("download " + path + ":" + error);
                    action?.Invoke(null, error);
                }
                else
                {
                    yield return new WaitForSeconds(delayTryTime);
                    yield return DownloadOne(path, action, canTryTimes, delayTryTime);
                }
                yield break;
            }
            action?.Invoke(request.downloadHandler, null);
            request.Dispose();
        }
        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="name">文件名 (内部会拼接完整路径)</param>
        /// <param name="data">数据</param>
        /// <returns>错误信息</returns>
        private string SaveData(string name, byte[] data)
        {
            try
            {
                File.WriteAllBytes(_destRootPath + name, data);
            }
            catch (Exception e)
            {
                Debug.LogError("save " + name + ":" + e.Message);
                return e.Message;
            }
            return "";
        }

        public static Queue<DownloadPair> BuildQueue(Dictionary<string, string> dic, bool useKey = false)
        {
            var queue = new Queue<DownloadPair>(dic.Count);
            foreach (var item in dic)
            {
                if (useKey)
                    queue.Enqueue(new DownloadPair(item.Key, item.Key));
                else
                    queue.Enqueue(new DownloadPair(item.Key, item.Value));
            }
            return queue;
        }
    }
}