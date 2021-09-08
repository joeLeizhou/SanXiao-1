
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Orcas.Networking
{
    [Serializable]
    public class ServerConfig : ScriptableObject
    {
        [Serializable]
        public enum ProtocolType {
            Http,
            Tcp,
            Udp
        }

        [SerializeField]
        private ProtocolType _protocolType = ProtocolType.Http;

        [SerializeField]
        private string _serverName = "UnNamedServer";

        [SerializeField]
        private List<UrlInfo> _urlList;

        [SerializeField]
        private string _url;

        [SerializeField]
        private int _port;

        [SerializeField]
        private int _defaultUrlIndex = 0;

        [SerializeField]
        private List<APIInfo> _apiList;

        [SerializeField]
        public TextAsset _decoder;

        public enum DecodeMethod {
            Default,
            Custom
        }

        public enum EncryptionMethod
        {
            Default,
            AES,
            Custom,
        }

        public string CustomDecoderClassName{ get; private set; }

        [SerializeField]
        private DecodeMethod _decodeMethod = DecodeMethod.Default;

        [SerializeField]
        private EncryptionMethod _encryptionMethod = EncryptionMethod.Default;
        
        private void OnValidate()
        {
            AdjustServerName();
            ValidateDecoder();
        }

        public void CopyData(ServerConfig newconfig)
        {
            newconfig._protocolType = _protocolType;
            newconfig._serverName = _serverName;
            newconfig._urlList = _urlList;
            newconfig._url = _url;
            newconfig._port = _port;
            newconfig._defaultUrlIndex = _defaultUrlIndex;
            newconfig._apiList = _apiList;
            newconfig._decodeMethod = _decodeMethod;
            newconfig._decoder = _decoder;
            newconfig.CustomDecoderClassName = CustomDecoderClassName;
            newconfig.name = _serverName;
        }

        public string GetServerName()
        {
            return _serverName;
        }

        public List<UrlInfo> GetServerUrlLists()
        {
            if (_urlList == null)
            {
                _urlList = new List<UrlInfo>();
            }
            return _urlList;
        }

        public DecodeMethod GetDecodeMethod()
        {
            return _decodeMethod;
        }

        public ProtocolType GetProtocolType()
        {
            return _protocolType;
        }

        public void AddUrlItem()
        {
            if (_urlList == null)
            {
                _urlList = new List<UrlInfo>();
            }

            var urlInfo = new UrlInfo();
            _urlList.Add(urlInfo);
        }

        public void deleteUrlItem(int index)
        {
            _urlList.RemoveAt(index);
        }

        public UrlInfo GetUrlInfo(int index)
        {
            if (_protocolType == ProtocolType.Http)
            {
                UrlInfo info = _urlList[index];
                return info;
            } else
            {
                UnityEngine.Debug.Log("Error! Please use getUrl() to get Url for Udp or Tcp protocol.");
                return null;
            }
        }

        public string GetUrl()
        {
            if (_protocolType != ProtocolType.Http)
            {
                return _url;
            } else
            {
                UnityEngine.Debug.Log("Error! Please use getUrlInfo() to get UrlInfo for Http protocol.");
                return null;
            }
        }

        public int GetPort()
        {
            if (_protocolType != ProtocolType.Http)
            {
                return _port;
            } else
            {
                UnityEngine.Debug.Log("Error! Please use getUrlInfo() to get UrlInfo for Http protocol.");
                return -1;
            }
        }

        public UrlInfo GetDefaultUrlInfo()
        {
            if (_protocolType == ProtocolType.Http)
            {
                if (_defaultUrlIndex < _urlList.Count && _defaultUrlIndex >= 0)
                {
                    UrlInfo info = _urlList[_defaultUrlIndex];
                    return info;
                } else
                {
                    UnityEngine.Debug.Log("Error! Default url does not exists");
                    return null;
                }
            }
            else
            {
                UnityEngine.Debug.Log("Error! Please use getUrl() to get Url for Udp or Tcp protocol.");
                return null;
            }
        }

        public void ResetDefaultUrlIndex()
        {
            _defaultUrlIndex = 0;
        }

        public int GetDefaultUrlIndex()
        {
            return _defaultUrlIndex;
        }

        public List<APIInfo> GetApiList() {
            if (_apiList == null)
            {
                _apiList = new List<APIInfo>();
            }
            return _apiList;
        }

        public void AddApiItem()
        {
            if (_apiList == null)
            {
                _apiList = new List<APIInfo>();
            }
            _apiList.Add(new APIInfo());
        }

        public void DeleteApiItem(int index)
        {
            _apiList.RemoveAt(index);
        }

        public void AdjustServerName()
        {
         
            if (_serverName.Length > 0)
            {
                if (_serverName[0] >= 97 && _serverName[0] <= 122)
                {
                    string newValue = "";
                    newValue += (char)(_serverName[0] - 32);

                    for (int i = 1; i < _serverName.Length; i++)
                    {
                        newValue += _serverName[i];
                    }

                    _serverName = newValue;
                }
            } else
            {
                if (name != "" && name[0] != ' ')
                {
                    _serverName = name;
                }
                else
                {
                    _serverName = "UnNamedServer";
                }
            }
        }

        private void ValidateDecoder()
        {
#if UNITY_EDITOR
            if (_decodeMethod == DecodeMethod.Default || _decoder == null) return;
            var guids = AssetDatabase.FindAssets(_decoder.name);
            if (guids.Length == 1)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

                if ((asset as MonoScript).GetClass().GetInterface(nameof(IDecoder)) == null)
                {             
                    _decoder = null;
                } else
                {
                    CustomDecoderClassName = (asset as MonoScript).GetClass().ToString();
                }
            }  
            else if (guids.Length > 1)
            {
                _decoder = null;
                UnityEngine.Debug.Log("Error! " + guids.Length + " decoders have the same name!");
            }   else
            {
                _decoder = null;
                UnityEngine.Debug.Log("Error! Can not find decoder named " + _decoder.name);
            }
#endif
        }

    }

}