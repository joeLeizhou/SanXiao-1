    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    namespace Orcas.Networking
    {
        public class SocketAddressManager
        {
            public const string SOCKET_ADDRESS_INFO_LIST = "SocketAddressInfoList";
            
            private static SocketAddressManager _instance;
            private Dictionary<string, IPAddressInfo> _map;

            public static SocketAddressManager Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new SocketAddressManager();
                    }

                    return _instance;
                }

                private set
                {
                    _instance = value;
                }
            }

            public List<IPAddressInfo> AddressInfos;

            private SocketAddressManager()
            {
                AddressInfos = new List<IPAddressInfo>();
                _map = new Dictionary<string, IPAddressInfo>();
                var list = LoadFromDisk();
                BuildList(list);
                BuildMap();
            }
            
            /// <summary>
            /// 过滤Tcp和Kcp
            /// </summary>
            /// <param name="list"></param>
            private void BuildList(IPAddressInfo[] list)
            {
                if(list == null || list.Length == 0) return;
                AddressInfos.Clear();
                for (int i = 0; i < list.Length; i++)
                {
                    var info = list[i];
                    var type = (ConnectType) info.Type;
                    if (type == ConnectType.Kcp || type == ConnectType.Tcp)
                    {
                        AddressInfos.Add(info);
                    }
                }
            }
            
            /// <summary>
            /// 设置服务器列表
            /// 以json数组形式的字符串（方便Lua使用)
            /// [{"ServerId":1,"Type":1,"Host":"162.14.11.141","Port":443,"AvgTime":0.0,"FailCount":0},{"ServerId":1,"Type":1,"Host":"162.14.11.141","Port":80,"AvgTime":0.0,"FailCount":0}]
            /// </summary>
            /// <param name="strJson"></param>
            public void SetServerList(string strJson)
            {
                if (string.IsNullOrEmpty(strJson)) return;
                var list = JsonArrayHelper.getJsonArray<IPAddressInfo>(strJson);
                SetServerList(list);
            }
            
            public void SetServerList(IPAddressInfo[] list)
            {
                List<IPAddressInfo> tempList = new List<IPAddressInfo>();
                if (list != null && list.Length > 0)
                {
                    int count1 = 0;
                    int count2 = 0;
                    for (int i = 0; i < list.Length; i++)
                    {
                        var info = list[i];
                        var type = (ConnectType) info.Type;
                        if (type == ConnectType.Kcp || type == ConnectType.Tcp)
                        {
                            string key = info.Host + ":" + info.Port;
                            // 有旧的记录就保持就的记录，没有用新的值
                            if (_map.ContainsKey(key))
                            {
                                tempList.Add(_map[key]);
                                count1++;
                            }
                            else
                            {
                                tempList.Add(info);
                                count2++;
                            }
                        }
                    }
                }
                AddressInfos = tempList;
                BuildMap();
                SaveToDisk();
            }
            
            /// <summary>
            /// 保存到本地
            /// </summary>
            public void SaveToDisk()
            {
                if (AddressInfos != null)
                {
                    var arr = AddressInfos.ToArray();
                    var wrap = new JsonArrayHelper.Wrapper<IPAddressInfo>();
                    wrap.array = arr;
                    string strJson = JsonUtility.ToJson(wrap);
                    PlayerPrefs.SetString(SOCKET_ADDRESS_INFO_LIST, strJson);
                    PlayerPrefs.Save();
                }
            }
            
            public IPAddressInfo[] LoadFromDisk()
            {
                string strJson = PlayerPrefs.GetString(SOCKET_ADDRESS_INFO_LIST, "");
                if (!string.IsNullOrEmpty(strJson))
                {
                    var list = JsonUtility.FromJson<JsonArrayHelper.Wrapper<IPAddressInfo>>(strJson);
                    return list?.array;
                }
                return default;
            }
            
            
            /// <summary>
            /// 以 Host:Port为key映射，方便取读
            /// </summary>
            private void BuildMap()
            {
                if(AddressInfos == null || AddressInfos.Count == 0) return;
                _map.Clear();
                for (int i = 0; i < AddressInfos.Count; i++)
                {
                    var info = AddressInfos[i];
                    string key = info.Host + ":" + info.Port;
                    if (_map.ContainsKey(key))
                    {
                        _map[key] = info;
                    }
                    else
                    {
                        _map.Add(key, info);
                    }
                }
            }
            
            /// <summary>
            /// 断线时调用，增加断线的次数统计
            /// </summary>
            /// <param name="host"></param>
            /// <param name="port"></param>
            public void AddSocketFailCount(string host, int port)
            {
                var key = host + ":" + port;
                if (_map.ContainsKey(key))
                {
                    _map[key].FailCount++;
                    SaveToDisk();
                }
            }

            public IPAddressInfo[] GetServerList()
            {
                return AddressInfos.ToArray();
            }
        }
    }
