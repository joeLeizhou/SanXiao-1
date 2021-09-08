using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Orcas.Networking;
using Orcas.Networking.Tcp;
using Orcas.Networking.Http;
using Orcas.Networking.Kcp;
using Orcas.Networking.Utilities;

namespace Orcas.Networking.NetClient
{
    public class NetClient : IClient, IClientEventHandler
    {
        public IClient Client { get; set; }
        public IClientEventHandler ClientEventHandler { get; set; }
        public IClientEventHandler MessageHandler { get; set; }

        public IChooseAddressStrategy ChooseAddressStrategy { get; set; }

        private NetworkState _networkState;
        public NetworkState NetworkState { get { return _networkState; } protected set { _networkState = value; } }

        private ClientOption _option;
        public ClientOption Option
        {
            get
            {
                return _option;
            }
            set
            {
                _option = value;
                if (Client != null) Client.Option = _option;
            }
        }
        public Channel Channel { get; set; }
        private bool _connected;
        private float _lastReceiveHeartTime = 0;
        private float _lastSendHeartTime = 0;
        private ReqHeartBeat _reqHeartHeat;
        private int _tryReconnectTimes = 0;
        private long _serverTime = 0;
        private long _serverStartLocalTime = 0;
        private ConnectType _currentConnectType;
        private bool _destroyed = false;
        private bool _hasReconnetTimeOut = false;
        private bool _isReconnecting = false;
        private string _host;
        private int _port;
        /// <summary>
        /// 多服务器选址时，Tcp配置
        /// </summary>
        private ClientOption _tcpOption;

        /// <summary>
        /// 多服务器选址时，Kcp配置
        /// </summary>
        private ClientOption _kcpOption;

        public NetClient()
        {
            NetworkUpdate.Instance.Init();
            _lastReceiveHeartTime = Time.realtimeSinceStartup;
            _lastSendHeartTime = Time.realtimeSinceStartup;
            _tryReconnectTimes = 0;
            _isReconnecting = false;
            _serverTime = 0;
            _serverStartLocalTime = 0;
            _destroyed = false;
            _hasReconnetTimeOut = false;
            NetworkState = NetworkState.DISCONNECTED;
            _reqHeartHeat = new DefaultReqHeartBeat();
            ChooseAddressStrategy = new SpeedTestChooseAddressStrategy();
        }

        private void OnUpdate()
        {
            var curTime = Time.realtimeSinceStartup;
            if (_isReconnecting && Option.NeedAutoReconnect && Option.NeedAutoReconnectTimeOut && curTime - _lastSendHeartTime > Option.ReconnectTimeOut)
            {
                DisConnect();
                _hasReconnetTimeOut = true;
                OnSocketException(SocketError.CanNotReconnectError, "Reconnect Time Out");
            }

            if (Client == null || NetworkState != NetworkState.CONNECTED) return;        // 没有初始化，或者正在连接
            if (!Option.NeedHeartBeat) return;                                           //配置认为不需要心跳

            if (Time.realtimeSinceStartup - _lastSendHeartTime > Option.HeartBeatInterval)
            {
                SendMessage(_reqHeartHeat);
                _lastSendHeartTime = curTime;
            }

            if (Time.realtimeSinceStartup - _lastReceiveHeartTime > Option.TimeOut)
            {
                _lastReceiveHeartTime += 1.0f;
                OnSocketException(SocketError.HeartBeatTimeOutError, "TimeOut");
            }
        }

        public void SendMessage(IReqProto message)
        {
            if (Client != null && Client.NetworkState == NetworkState.CONNECTED)
            {
                Client.SendMessage(message);
            }
        }


        public void SendLuaMessage(ushort ID, string Data)
        {
            try
            {
                if (Client != null && Client.NetworkState == NetworkState.CONNECTED)
                {
                    ReqLuaMessage msg = new ReqLuaMessage();
                    msg.ID = ID;
                    msg.Data = Data;
                    Client.SendMessage(msg);
                }
                else
                {
                    Debug.LogError("not connected");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void SetChooseServerStrategy(IChooseAddressStrategy strategy)
        {
            ChooseAddressStrategy = strategy;
        }

        public void MultiSocketConnect(ClientOption optionTcp = null, ClientOption optionKcp = null)
        {
            _tcpOption = optionTcp;
            _kcpOption = optionKcp;
            var address = ChooseAddressStrategy.ChooseAddress(SocketAddressManager.Instance.GetServerList());
            if (address != null)
            {
                var addressType = (ConnectType)address.Type;
                var clientOption = addressType == ConnectType.Kcp ? _kcpOption : _tcpOption;
                Connect(addressType, address.Host, address.Port, clientOption);
            }
            else
            {
                Debug.LogError("[NetClient] 选址失败");
            }
        }


        public void MultiSocketConnect(string strJson, ClientOption optionTcp = null, ClientOption optionKcp = null)
        {
            SocketAddressManager.Instance.SetServerList(strJson);
            MultiSocketConnect(optionTcp, optionKcp);
        }


        public void Connect(ConnectType type, string ip, int port, ClientOption option = null)
        {
            _currentConnectType = type;
            _host = ip;
            _port = port;
            _destroyed = false;

            // 防止重复添加
            NetworkUpdate.Instance.UpdateEvent -= OnUpdate;
            NetworkUpdate.Instance.UpdateEvent += OnUpdate;
            NetworkUpdate.Instance.DestoryEvent -= OnDestroy;
            NetworkUpdate.Instance.DestoryEvent += OnDestroy;

            SetClientOption(option);

            if (Client != null)
            {
                Client.DisConnect();
            }
            if (type == ConnectType.Tcp)
            {
                Client = new TcpClient(ip, port) { ClientEventHandler = this };
            }
            else if (type == ConnectType.Http)
            {
                var url = $"http://{ip}:{port}";
                Client = new HttpClient(url) { ClientEventHandler = this };
            }
            else if (type == ConnectType.Https)
            {
                var url = $"https://{ip}:{port}";
                Client = new HttpClient(url) { ClientEventHandler = this };
            }
            else if (type == ConnectType.Kcp)
            {
                Client = new KcpClient(ip, port) { ClientEventHandler = this };
            }

            StartConnect();
        }

        private async void StartConnect()
        {
            _hasReconnetTimeOut = false;
            NetworkState = NetworkState.CONNECTING;
            _connected = await Client.Connect();
            await Task.Delay(20); // OnConnectedListener 里面立即发消息可能会报错，加个延时
            OnConnectResult(_connected);
            if (_connected == false)
            {
                Debug.Log("connect failed!!!");
                return;
            }
            Debug.Log("connect success!!!");
        }

        private bool CheckClientEventHandler()
        {
            return ClientEventHandler == null ? false : true;
        }

        public Task<bool> Connect()
        {
            Debug.LogError("[NetClient] 顶层NetClient需要参数进行连接初始化，请调用带参数的Connect接口。");
            return Task.FromResult<bool>(false);
        }

        public void DisConnect()
        {
            OnDestroy();
        }

        /// <summary>
        /// 主要防止关闭游戏后还调起（Editor出现过）
        /// </summary>
        public void OnDestroy()
        {
            if (NetworkUpdate.Instance)
            {
                NetworkUpdate.Instance.UpdateEvent -= OnUpdate;
                NetworkUpdate.Instance.DestoryEvent -= OnDestroy;
            }
            if (Client != null)
            {
                Client.DisConnect();
                Client.ClientEventHandler = null;
            }
            Client = null;
            _destroyed = true;
        }

        public void SetClientOption(ClientOption option)
        {
            if (option == null)
            {
                option = new ClientOptionBuilder(_currentConnectType).Build();
            }

            Option = option;
            if (Client != null) Client.Option = Option;
            if (Option != null)
            {
                _reqHeartHeat.ID = Option.ReqHeartBeatID;
                ProtocolFactory factory = ProtocolFactory.Instance;
                factory.AddProto<DefaultReqHeartBeat>(Option.ReqHeartBeatID);
                factory.AddProto<DefaultRltHeartBeat>(Option.RltHeartBeatID);
            }
        }



        public void OnReceiveMessage(IRltProto proto)
        {
            NetworkUpdate.Instance.AddActionDoInMainThread(() =>
            {
                if (Option.NeedHeartBeat && proto.ID == Option.RltHeartBeatID)
                {
                    OnReceiveHeartBeat((RltHeartBeat)proto);
                }
                MessageHandler?.OnReceiveMessage(proto);
            });
        }

        private void OnReceiveHeartBeat(RltHeartBeat proto)
        {
            _serverTime = proto.Time;
            _lastReceiveHeartTime = Time.realtimeSinceStartup;
            _serverStartLocalTime = DateTime.UtcNow.Ticks;
        }

        public void OnReceiveLuaMessage(RltLuaMessage proto)
        {
            NetworkUpdate.Instance.AddActionDoInMainThread(() =>
            {
                MessageHandler?.OnReceiveLuaMessage(proto);
            });
        }

        public void OnSocketException(SocketError error, string message)
        {
            NetworkUpdate.Instance.AddActionDoInMainThread(() =>
            {
                _connected = false;
                NetworkState = NetworkState.DISCONNECTED;
                // 先回调再重连，保证顺序
                MessageHandler?.OnSocketException(error, message);

                // ConnectFailedError 由ConnectResult处理
                if (error != SocketError.CanNotReconnectError && error != SocketError.ConnectFailedError)
                {
                    TryAutoReconnect();
                }

                if (error == SocketError.CanNotReconnectError)
                {
                    _isReconnecting = false;
                }

                // 记录断线次数，作为多服务器选服参考
                SocketAddressManager.Instance.AddSocketFailCount(_host, _port);
            });
        }

        private void TryAutoReconnect()
        {
            NetworkState = NetworkState.DISCONNECTED;
            if (!_isReconnecting && Option.NeedAutoReconnect && Option.NeedAutoReconnectTimeOut)
            {
                // 如果是非重连就超时，就重置，以触发第一次重连
                _lastSendHeartTime = Time.realtimeSinceStartup;
                _lastReceiveHeartTime = Time.realtimeSinceStartup;
            }

            // 连接失败视情况继续尝试重连
            // 1、如果非超时模式，则按超时次数
            // 2、如果是超时模式，按超时时间
            if (Option.NeedAutoReconnect && ((!Option.NeedAutoReconnectTimeOut && _tryReconnectTimes < Option.MaxReconnectCount)
                                             || (Option.NeedAutoReconnectTimeOut && Time.realtimeSinceStartup - _lastSendHeartTime <= Option.ReconnectTimeOut)))
            {
                TryReconnect(false);
            }

            // 重试次数超了，不再连接
            if (_isReconnecting && !Option.NeedAutoReconnectTimeOut && _tryReconnectTimes == Option.MaxReconnectCount)
            {
                // 达到重试限制，抛出无法连接错误
                OnSocketException(SocketError.CanNotReconnectError, "can not reconnect");
            }
        }

        public async void TryReconnect(bool reset = false)
        {
            _isReconnecting = true;
            if (reset) _tryReconnectTimes = 0;
            if (Option.NeedAutoReconnect)
            {
                _tryReconnectTimes++;
                StartConnect();
            }
        }

        public void OnConnectResult(bool success)
        {
            if (_destroyed) return;
            if (Option.NeedAutoReconnect && Option.NeedAutoReconnectTimeOut && _hasReconnetTimeOut) return;

            NetworkUpdate.Instance.AddActionDoInMainThread(() =>
            {
                MessageHandler?.OnConnectResult(success);
                if (success)
                {
                    // 重连成功之后重置尝试次数
                    _tryReconnectTimes = 0;
                    _lastReceiveHeartTime = Time.realtimeSinceStartup;
                    _lastSendHeartTime = Time.realtimeSinceStartup;
                    NetworkState = NetworkState.CONNECTED;
                    _isReconnecting = false;
                }
                else
                {
                    TryAutoReconnect();
                }
            });
        }
    }
}
