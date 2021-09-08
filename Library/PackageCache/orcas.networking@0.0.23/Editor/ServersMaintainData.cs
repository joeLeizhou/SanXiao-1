#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    [Serializable]
    public class ServersMaintainData : ScriptableObject
    {
        //l里面要存储的属性必须全部标明序列化
        [Serializable]
        public struct APIBrief
        {
            [SerializeField]
            public string APIName;
            [SerializeField]
            public string Path;

            [SerializeField]
            public APIInfo.Format Format;
            public APIBrief(string apiName, string path, APIInfo.Format format)
            {
                APIName = apiName;
                Path = path;
                Format = format;
            }
        }

        [Serializable]
        public struct ServerBrief
        {
            [SerializeField]
            public string ServerName;
            [SerializeField]
            public List<APIBrief> APIBriefList;

            public ServerBrief(string serverName, List<APIBrief> apiBriefList)
            {
                ServerName = serverName;
                APIBriefList = apiBriefList;
                Debug.Log("APIBriefList count: " + APIBriefList.Count);
            }
        }

        [SerializeField]
        private List<ServerBrief> _serverBriefs;

        public void AddServerBrief(string serverName, List<APIBrief> apiBriefList)
        {
            if (_serverBriefs == null)
            {
                _serverBriefs = new List<ServerBrief>();
            }

            for (int i = 0; i < _serverBriefs.Count; i++)
            {
                var item = _serverBriefs[i];
                if (item.ServerName == serverName)
                {
                    _serverBriefs.RemoveAt(i);
                    break;
                }
            }

            Debug.Log("apiBriefList count: " + apiBriefList.Count);

            _serverBriefs.Add(new ServerBrief(serverName, apiBriefList));
        }

        public bool DeleteServerBrief(string serverName)
        {
            for (int i = 0; i < _serverBriefs.Count; i++)
            {
                var item = _serverBriefs[i];
                if (item.ServerName == serverName)
                {
                    _serverBriefs.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void ClearTheList()
        {
            _serverBriefs.Clear();
        }

        public List<ServerBrief> GetBriefs()
        {
            if (_serverBriefs == null)
            {
                _serverBriefs = new List<ServerBrief>();
            }
            return _serverBriefs;
        }

        public bool ContainsServer(string serverName)
        {
            if (_serverBriefs == null)
            {
                _serverBriefs = new List<ServerBrief>();
            }

            for (int i = 0; i < _serverBriefs.Count; i++)
            {
                if (_serverBriefs[i].ServerName == serverName) return true;
            }

            return false;
        }

        public (bool, string) ContainsApi(string apiName, APIInfo.Format format)
        {
            for (int i = 0; i < _serverBriefs.Count; i++)
            {
                var serverBrief = _serverBriefs[i];
                var apiBriefList = serverBrief.APIBriefList;
                for (int j = 0; j < apiBriefList.Count; j++)
                {
                    var apiBrief = apiBriefList[j];
                    if (apiBrief.APIName == apiName && (apiBrief.Format == APIInfo.Format.Both || apiBrief.Format == format))
                    {
                        return (true, serverBrief.ServerName);
                    }
                }
            }

            return (false, "");
        }

        public List<APIBrief> GetServerApiBriefList(string serverName)
        {
            for (int i = 0; i < _serverBriefs.Count; i++)
            {
                var serverBrief = _serverBriefs[i];
                if (serverBrief.ServerName == serverName)
                {
                    return serverBrief.APIBriefList;
                }
            }

            return null;
        }

        public void SetServerApiBriefList(string serverName, List<APIBrief> apiBriefList)
        {
            for (int i = 0; i < _serverBriefs.Count; i++)
            {
                if (_serverBriefs[i].ServerName == serverName)
                {
                    _serverBriefs[i].APIBriefList.Clear();
                    _serverBriefs[i].APIBriefList.AddRange(apiBriefList);
                }
            }
        }

        public List<string> GetServerNameList()
        {
            var serverNameList = new List<string>();
            foreach (var serverBrief in _serverBriefs)
            {
                serverNameList.Add(serverBrief.ServerName);
            }
            return serverNameList;
        }
    }
}
#endif