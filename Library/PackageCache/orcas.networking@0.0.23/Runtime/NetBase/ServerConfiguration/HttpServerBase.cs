using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using Orcas.Core;

namespace Orcas.Networking
{

    public class RequestState
    {
        public int requestID;
        public Action<IResponseData, int> callBack;
    }
    public class HttpServerBase
    {
        public Dictionary<string, string> _requestHeader = new Dictionary<string, string>();
        private bool destroyed = false;

        private Dictionary<int, HttpWebRequest> requiresMap = new Dictionary<int, HttpWebRequest>();
        public int RequestID { get; private set; } = 0;
        public string DefaultUrl;
        public IDecoder Decoder; 

        public ServerConfig GetConfigFromPath(string serverConfigPath) 
        {
#if UNITY_EDITOR
           return AssetDatabase.LoadAssetAtPath<ServerConfig>(serverConfigPath);
#else
           return Resources.Load<ServerConfig>(serverConfigPath);
#endif
        }
        public void Destroy()
        {
            destroyed = true;
        }

        public void AddHeaderProperty(KeyValuePair<string, string >[] pairs)
        {
            foreach (var pair in pairs) {
                if (_requestHeader.ContainsKey(pair.Key))
                    _requestHeader.Remove(pair.Key);
                _requestHeader.Add(pair.Key, pair.Value);
            }
        }

        public void ClearHeader()
        {
            _requestHeader.Clear();
        }

        public void RemoveHeaderPeoperty(string key)
        {
            if (_requestHeader.ContainsKey(key))
                _requestHeader.Remove(key);
        }

        private Dictionary<string, string> GenerateFormByParams(string args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(args))
            {
                return result;
            }
            string[] strArr = args.Split('&');
            foreach (string str in strArr)
            {
                Debug.Log("args===>" + str);
                string[] kvs = str.Split('=');
                if (kvs != null && kvs.Length > 1)
                {
                    string key = kvs[0];
                    string value = kvs[1];
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        result.Add(key, value);
                    }
                    Debug.Log("key===>" + key + " value===>" + value);
                }
            }
            return result;
        }

        public int SendUnityWebRequest(string path, string args, Action<IResponseData, int> callBack, bool isPost)
        {
            var requestID = RequestID++;
            string url = DefaultUrl;
            Unity_UrlRequire(url, path, args, callBack, requestID, isPost);
            return requestID;
        }

        public int SendUnityFormWebRequest(string path, string args, Action<IResponseData, int> callBack)
        {
            var requestID = RequestID++;
            string url = DefaultUrl;
            Unity_FormRequire(url, path, args, callBack, requestID);
            return requestID;
        }

        public int SendUnityPostWWWFormWebRequest(string path, string args, Action<IResponseData, int> callBack)
        {
            var requestID = RequestID++;
            string url = DefaultUrl;
            Unity_WWWFormRequire(url, path, args, callBack, requestID);
            return requestID;
        }

        private void Unity_UrlRequire(string url, string path, string args, Action<IResponseData, int> callBack, int requestID, bool isPost)
        {
            if (destroyed) return;
            Debug.Log("before CoroutineManager call StartCoroutine");
            CoroutineManager.Instance.StartCoroutine(IUnity_UrlRequire(url, path, args, callBack, requestID, isPost));
        }

        private IEnumerator IUnity_UrlRequire(string url, string path, string args, Action<IResponseData, int> callBack, int requestID, bool isPost)
        {
            if (string.IsNullOrEmpty(url))
                yield return null;
            url += !string.IsNullOrEmpty(path) ? ("/" + path) : string.Empty;
            url += !string.IsNullOrEmpty(args) ? ("?" + args) : string.Empty;
            Debug.Log("before IC_Require create request");

            UnityWebRequest request = isPost ? UnityWebRequest.Post(url, string.Empty) : UnityWebRequest.Get(url);

            Debug.Log("after IC_Require create request");

            /**
             * 添加header
             */
#if UNITY_EDITOR
            request.SetRequestHeader("OS-TYPE", "editor");
#elif UNITY_ANDROID
        request.SetRequestHeader("OS-TYPE", "android");
#else
        request.SetRequestHeader("OS-TYPE", "ios");
#endif
            foreach (KeyValuePair<string, string> kvp in _requestHeader)
            {
                request.SetRequestHeader(kvp.Key, kvp.Value);
            }
            Debug.Log("after IC_Require set request header");

#if ADDLOG
        DebugHelper.Log("UnityRequire 11 , url =" + url);
#endif

#if UNITY_2017_1_OR_NEWER
            yield return request.SendWebRequest();
#else
        yield return request.Send();
#endif
            var defaultDecoder = new DefaultDecoder();
            var iResponseData = defaultDecoder.Decode((int)request.responseCode, request.downloadHandler.text);
            callBack(iResponseData, requestID);
#if ADDLOG
            DebugHelper.log("UnityRequire" + request.url + " -> " + strResponse);
#endif
            request.Dispose();
        }

        private void Unity_FormRequire(string url, string path, string args, Action<IResponseData, int> callBack, int requestID)
        {
            if (destroyed) return;
            CoroutineManager.Instance.StartCoroutine(IUnity_FormRequire(url, path, args, callBack, requestID));
        }

        private IEnumerator IUnity_FormRequire(string url, string path, string args, Action<IResponseData, int> callBack, int requestID)
        {
#if ADDLOG
        DebugHelper.Log("IPost_FormRequire  , url =" + url + ", args = " + args);
#endif
            if (string.IsNullOrEmpty(url))
            {
                yield return null;
            }

            var form = GenerateFormByParams(args);

            url += !string.IsNullOrEmpty(path) ? ("/" + path) : string.Empty;

            UnityWebRequest request = UnityWebRequest.Post(url, form);

            /**
             * 添加header
             */
#if UNITY_EDITOR
            request.SetRequestHeader("OS-TYPE", "editor");
#elif UNITY_ANDROID
        request.SetRequestHeader("OS-TYPE", "android");
#else
        request.SetRequestHeader("OS-TYPE", "ios"); 
#endif
            foreach (KeyValuePair<string, string> kvp in _requestHeader)
            {
                request.SetRequestHeader(kvp.Key, kvp.Value);
            }

#if UNITY_2017_1_OR_NEWER
            yield return request.SendWebRequest();
#else
        yield return request.Send();
#endif
           
            var iResponseData = Decoder.Decode((int)request.responseCode, request.downloadHandler.text);
            callBack(iResponseData, requestID);

            request.Dispose();
#if ADDLOG
        DebugHelper.Log("IPost_FormRequire ,  strResponse = " + strResponse);
#endif
        }

        private void Unity_WWWFormRequire(string url, string path, string args, Action<IResponseData, int> callBack, int requestId)
        {
            if (destroyed) return;
            CoroutineManager.Instance.StartCoroutine(IUnity_WWWFormRequire(url, path, args, callBack, requestId));
        }

        private IEnumerator IUnity_WWWFormRequire(string url, string path, string args, Action<IResponseData, int> callBack, int requestId)
        {
#if ADDLOG
        DebugHelper.Log("UnityRequire 0 , url =" + url + ", args = " + args);
#endif
            if (string.IsNullOrEmpty(url))
            {
                yield return null;
            }

            Dictionary<string, string> parametersDictionary = GenerateFormByParams(args);

            WWWForm form = new WWWForm();

            foreach (KeyValuePair<string, string> kvp in parametersDictionary)
            {
                form.AddField(kvp.Key, kvp.Value);
            }

            url += !string.IsNullOrEmpty(path) ? ("/" + path) : string.Empty;

            UnityWebRequest request = UnityWebRequest.Post(url, form);

            /**
             * 添加header
             */
#if UNITY_EDITOR
            request.SetRequestHeader("OS-TYPE", "editor");
#elif UNITY_ANDROID
        request.SetRequestHeader("OS-TYPE", "android");
#else
        request.SetRequestHeader("OS-TYPE", "ios"); 
#endif
            foreach (KeyValuePair<string, string> kvp in _requestHeader)
            {
                request.SetRequestHeader(kvp.Key, kvp.Value);
            }

#if UNITY_2017_1_OR_NEWER
            yield return request.SendWebRequest();
#else
        yield return request.Send();
#endif

            var iResponseData = Decoder.Decode((int)request.responseCode, request.downloadHandler.text);
            callBack(iResponseData, requestId);
            request.Dispose();
#if ADDLOG
        DebugHelper.Log("UnityRequire ,  strResponse = " + strResponse);
#endif
        }
        private  IEnumerator IUnity_FormRequirePlus(string[] url, string method, string args, Action<IResponseData, int> callBack, int requestId)
        {
            int tryAfterFailedTimes = 0;
            //int urlIndex = HttpManager.httpUrlIndex >= url.Length ? 0 : HttpManager.httpUrlIndex;
            int urlIndex = 0;
            // DebugHelper.Log("UnityRequire 0 , url =" + url);

            bool needRetry = false;
            string strResponse = string.Empty;

            IResponseData iResponseData = null;

            do
            {
                if (tryAfterFailedTimes >= 3 || tryAfterFailedTimes >= url.Length)
                    break;
                if (tryAfterFailedTimes > 0)
                    yield return new WaitForSeconds(0.1f);
                string urlStr = url[urlIndex];
                strResponse = string.Empty;
                if (string.IsNullOrEmpty(urlStr))
                {
                    yield return null;
                    urlIndex++;
                    continue;
                }
#if ADDLOG
            DebugHelper.Log("IPost_FormRequire  , url =" + urlStr + ", args = " + args);
#endif
                var form = GenerateFormByParams(args);

                urlStr += !string.IsNullOrEmpty(method) ? ("/" + method) : string.Empty;

                UnityWebRequest request = UnityWebRequest.Post(urlStr, form);

                /**
                * 添加header
                */
#if UNITY_EDITOR
                request.SetRequestHeader("OS-TYPE", "editor");
#elif UNITY_ANDROID
            request.SetRequestHeader("OS-TYPE", "android");
#else
            request.SetRequestHeader("OS-TYPE", "ios"); 
#endif
                foreach (KeyValuePair<string, string> kvp in _requestHeader)
                {
                    request.SetRequestHeader(kvp.Key, kvp.Value);
                }

#if UNITY_2017_1_OR_NEWER
                yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif

#if UNITY_2017_1_OR_NEWER
                if (request.isHttpError || request.isNetworkError)
#else
            if (request.isError)
#endif
                {
                    needRetry = false;
                    
                    iResponseData = Decoder.Decode((int)request.responseCode, request.downloadHandler.text);
                    
                    if (request.responseCode == 0 || (request.responseCode >= 500 && request.responseCode < 600))
                    {
                        needRetry = true;
                        urlIndex = ++urlIndex >= url.Length ? 0 : urlIndex;
                    }
#if ADDLOG
                DebugHelper.Log("IPost_FormRequire error, ~~ " + request.error + "  urlStr: " + urlStr + "  request.responseCode: " + request.responseCode + "  urlIndex: " + urlIndex);
#endif
                }
                else
                {
#if ADDLOG
                DebugHelper.Log("IPost_FormRequire Success,  result = " + request.downloadHandler.text);
#endif
                    iResponseData = Decoder.Decode((int)request.responseCode, request.downloadHandler.text);
                   
                    
                    needRetry = false;
                    strResponse = request.downloadHandler.text;

                }
                
                request.Dispose();
                
            }
            while (needRetry && ++tryAfterFailedTimes < 3 && tryAfterFailedTimes < url.Length);
            callBack(iResponseData, requestId);
            //callBack(iResponseData, requestId);
#if ADDLOG
        DebugHelper.Log("IPost_FormRequire ,  strResponse = " + strResponse);
#endif

        }

        private void CallBack(IAsyncResult asynchronousResult)
        {
            RequestState requestState = (RequestState)asynchronousResult.AsyncState;
            if (requiresMap.ContainsKey(requestState.requestID) && requiresMap[requestState.requestID] != null)
            {
                HttpWebResponse response = (HttpWebResponse)requiresMap[requestState.requestID].EndGetResponse(asynchronousResult);
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string resultString = streamReader.ReadToEnd();
                        if (resultString == null)
                        {
                            return;
                        }

                       
                        var iResponseData = Decoder.Decode((int)HttpStatusCode.OK, resultString);
                        var callBack = requestState.callBack;
                        callBack(iResponseData, requestState.requestID);
                        requiresMap.Remove(requestState.requestID);
                    }
                }
            }
        }
    }
}