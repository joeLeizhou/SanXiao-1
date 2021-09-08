using Orcas.Core;
using Orcas.Core.Tools;
using Orcas.Game.Multiplayer;
using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;
using Orcas.Networking.Tcp;
using Orcas.Networking.Kcp;
using Orcas.Networking.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Orcas.Networking.NetClient;

namespace Orcas.Game.Common
{
    public class GameClientManager : IManager, IClientEventHandler
    {
        public NetClient Client { get; private set; }
        public ILoginHandler LoginHandler { get; set; }
        public IMatcher Matcher { get; set; }

        public IClientEventHandler MessageHandler { get; set; }

        private readonly Dictionary<ushort, Action<IRltProto>> _protoCallBackDictionary = new Dictionary<ushort, Action<IRltProto>>(32);
        private Action<bool, bool> OnConnectedListener;

        public void Connect(ConnectType type, string ip, int port, Action<bool, bool> callback, ILoginHandler loginHandler = null)
        {
            if (Client != null)
            {
                Client.DisConnect();
            }

            Client = new NetClient();
            var clientOption = new ClientOptionBuilder(type)
                .SetAutoReconnectParams(true, 3, false, 0)
                .SetHeartBeatProtocols(CommonProtoId.ReqBHeartBeat, CommonProtoId.RltBHeartBeat)
                .Build();
            Client.MessageHandler = this;
            LoginHandler = loginHandler;
            if (LoginHandler != null) LoginHandler.Client = Client;
            OnConnectedListener = callback;
            Client.Connect(type, ip, port, clientOption);

            // 获取IP地址的国家信息
            GetCountryCode();
        }

        public void OnConnectResult(bool success)
        {
            MessageHandler?.OnConnectResult(success);
            OnConnectedListener?.Invoke(success, false);
        }

        public void OnReceiveMessage(IRltProto proto)
        {
            switch (proto.ID)
            {
                case CommonProtoId.RltLogin:
                    LoginHandler?.OnLogined((RltLogin)proto);
                    break;
                case CommonProtoId.RltMatch:
                    Matcher?.OnMatch((RltMatch)proto);
                    break;
                case CommonProtoId.RltMatchInfo:
                    Matcher?.OnMatchInfo((RltMatchInfo)proto);
                    break;
                case CommonProtoId.RltCancelMatch:
                    Matcher?.OnMatchCancel((RltCancelMatch)proto);
                    break;
                case CommonProtoId.RltBHeartBeat:
                    OnReceiveHeartBeat((RltHeartBeat)proto);
                    break;
                case CommonProtoId.RltServerList:
                    OnReceiveServerGroup((RltServerList)proto);
                    break;
            }
            proto.Deal();

            if (_protoCallBackDictionary.TryGetValue(proto.ID, out var callback))
            {
                callback.Invoke(proto);
            }
            MessageHandler?.OnReceiveMessage(proto);
        }
        public RltServerList ServerList { get; private set; }
        private void OnReceiveServerGroup(RltServerList proto)
        {
            ServerList = proto;
        }

        private long _serverTime = 0;
        private long _serverStartLocalTime = 0;
        private void OnReceiveHeartBeat(RltHeartBeat proto)
        {
            _serverTime = proto.Time;
            _serverStartLocalTime = DateTime.UtcNow.Ticks;
        }
        public long GetServerTimeLong()
        {
            return ((DateTime.UtcNow.Ticks - _serverStartLocalTime) / TimeSpan.TicksPerMillisecond) + _serverTime;
        }
        public float GetServerTime()
        {
            return (float)GetServerTimeLong();
        }
        public void SetRltProtoEvent(ushort protoId, Action<IRltProto> callback)
        {
            _protoCallBackDictionary[protoId] = callback;
        }

        public void RemoveRltProtoEvent(ushort protoId)
        {
            _protoCallBackDictionary.Remove(protoId);
        }

        public void OnSocketException(SocketError error, string message)
        {
            MessageHandler?.OnSocketException(error, message);
        }


        public void SendMessage(IReqProto message)
        {
            Client?.SendMessage(message);
        }

        public void SendLuaMessage(ushort ID, string Data)
        {
            Client?.SendLuaMessage(ID, Data);
        }

        private int _tryReconnectTimes = 0;

        public void Init()
        {
            AddProtos();
        }
        private void AddProtos()
        {
            // login
            ProtocolFactory factory = ProtocolFactory.Instance;
            factory.AddProto<ReqLogin>(CommonProtoId.ReqLogin);
            factory.AddProto<RltLogin>(CommonProtoId.RltLogin);
            factory.AddProto<RltConfig>(CommonProtoId.RltConfig, true, true);
            factory.AddProto<RltServerList>(CommonProtoId.RltServerList);
        }

        public void OnDestroy()
        {
            Client?.DisConnect();
            Client = null;
        }

        public void OnPause()
        {

        }

        public void OnResume()
        {

        }

        public void Update(uint currentFrameCount)
        {

        }

        public void OnReceiveLuaMessage(RltLuaMessage proto)
        {
            MessageHandler?.OnReceiveLuaMessage(proto);
        }


        #region country code
        public class CountryData
        {
            public string ip;
            public string countryCode;
            public string countryName;
            public string cityName;
        }

        public static void GetCountryCode()
        {
            CoroutineManager.Instance.StartCoroutine(RequestCountryCode());
        }

        public static IEnumerator RequestCountryCode()
        {
            // using (UnityWebRequest request = UnityWebRequest.Get("http://geoip.fotoable.com"))
            using (UnityWebRequest request = UnityWebRequest.Get("https://geoip.fotoable.com"))
            {
                yield return request.SendWebRequest();
                if (request.isHttpError || request.isNetworkError)
                {
                    UnityEngine.Debug.Log(request.error);
                }
                else
                {
                    try
                    {
                        UnityEngine.Debug.Log("get geo" + request.downloadHandler.text);
                        CountryData obj = JsonUtility.FromJson<CountryData>(request.downloadHandler.text);
                        if (obj != null && string.IsNullOrEmpty(obj.countryCode) == false)
                            PlayerPrefsManager.SetString(PlayerPrefHelper.CountryCode, obj.countryCode);
                    }
                    catch (System.Exception ex)
                    {
                        UnityEngine.Debug.LogError("RequestCountryCode " + ex.ToString());
                    }
                }
            }
        }
        #endregion
    }
}
