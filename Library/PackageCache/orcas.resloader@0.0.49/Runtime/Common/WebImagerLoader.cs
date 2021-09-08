using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Orcas.Core;

namespace Orcas.Resources
{
    public class WebImagerLoader
    {
        //缓存结果dic
        private static Dictionary<string, Texture> cacheUrlMap = new Dictionary<string, Texture>(1 << 3);
        //正在请求中的url
        private static Dictionary<string, bool> cacheUrlRequiringMap = new Dictionary<string, bool>(1 << 3);
        //最大下载数量
        private static readonly int MAX_DOWNLOAD_COUNT = 10;
        //当前下载数量
        private static int _currentDownloadCount = 0;
        private static Queue<LoadWebTextureReq> loadWebTextureReqsQue = null;
        private static int id = 0;

        /// <summary>
        /// 下载远端Texture
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static int LoadWebTexture(string url, System.Action<Texture, int> action)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("no url");
                return id;
            }
            LoadWebTextureReq req = new LoadWebTextureReq(id, url, action);
            LoadWebTexture(req);
            return id++;
        }

        /// <summary>
        /// 下载远端Sprite
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static int LoadWebSprite(string url, System.Action<Sprite, int> action)
        {
            LoadWebTexture(url, (tex, _id) =>
            {
                Texture2D te = (Texture2D)tex;
                Sprite sprite = Sprite.Create(te, new Rect(0, 0, te.width, te.height), new Vector2(0.5f, 0.5f));
                if (sprite != null) action(sprite, _id);
            });
            return id++;
        }
        
        
        private static void LoadWebTexture(LoadWebTextureReq req)
        {
            if (cacheUrlMap.ContainsKey(req.url) == true && cacheUrlMap[req.url] != null)
            {

                req.action(cacheUrlMap[req.url], req.id);
                return;
            }
            if (_currentDownloadCount < MAX_DOWNLOAD_COUNT)
            {
                CoroutineManager.Instance.StartCoroutine(ILoadWebTexture(req));
            }
            else
            {
                if (loadWebTextureReqsQue == null)
                {
                    loadWebTextureReqsQue = new Queue<LoadWebTextureReq>();
                }
                loadWebTextureReqsQue.Enqueue(req);
            }
        }

        private static IEnumerator ILoadWebTexture(LoadWebTextureReq req)
        {
            if (cacheUrlRequiringMap.ContainsKey(req.url) == true)
            {
                while (!cacheUrlMap.ContainsKey(req.url))
                {
                    yield return null;
                }
                var t = cacheUrlMap[req.url];
                Debug.Log("get texture from same url" + req.url);
                req.action?.Invoke(cacheUrlMap[req.url], req.id);
            }
            else
            {
                cacheUrlRequiringMap.Add(req.url, true);
                _currentDownloadCount++;
                if (req.url.Length == 0) yield break;
                UnityWebRequest webReq = new UnityWebRequest(req.url);
                DownloadHandlerTexture texHandler = new DownloadHandlerTexture(true);
                webReq.downloadHandler = texHandler;
                yield return webReq.SendWebRequest();
                cacheUrlRequiringMap.Remove(req.url);
                _currentDownloadCount--;
                if (loadWebTextureReqsQue != null && loadWebTextureReqsQue.Count > 0)
                {
                    LoadWebTexture(loadWebTextureReqsQue.Dequeue());
                }
                if (webReq.isNetworkError || webReq.isHttpError)
                {
                    Debug.LogWarning("download texture failed：" + webReq.error + " " + req.url);
                    yield break;
                }
                var tex = texHandler.texture;
                if (tex && !cacheUrlMap.ContainsKey(req.url))
                {
                    cacheUrlMap.Add(req.url, tex);
                }
                req.action?.Invoke(cacheUrlMap[req.url], req.id);
            }
        }

        private struct LoadWebTextureReq
        {
            public string url;
            public System.Action<Texture, int> action;
            public int id;
            public LoadWebTextureReq(int id, string url, System.Action<Texture, int> action)
            {
                this.id = id;
                this.url = url;
                this.action = action;
            }
        }
    }
}
