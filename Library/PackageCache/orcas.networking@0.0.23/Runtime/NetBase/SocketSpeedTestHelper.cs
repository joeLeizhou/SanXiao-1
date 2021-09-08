using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Networking.NetClient;

namespace Orcas.Networking
{
    public class SocketSpeedTestHelper : IClientEventHandler
    {
        private bool _hasLoaded = false;

        /// <summary>
        /// 每个ip地址测试几次，即向服务器ping几次
        /// </summary>
        public int TestCount = 3;

        /// <summary>
        /// 客户端向服务器发送测速协议的ID
        /// </summary>
        public ushort ReqTestSpeedProtoId = 18002;

        
        /// <summary>
        /// 服务器返回客户端测速协议的ID
        /// </summary>
        public ushort RltTestSpeedProtoId = 18003;
        
        /// <summary>
        /// Tcp参数
        /// </summary>
        public ClientOption TcpOption;

        /// <summary>
        /// Kcp参数
        /// </summary>
        public ClientOption KcpOption;
        
        
        private NetClient.NetClient _client;

        private int _currentHostIndex = 0;

        private int _currentCount = 0;

        private ReqTestSpeed _testSpeedProto;

        private long _lastSendTime;

        public SocketSpeedTestHelper(string serverList, ushort reqId = 18002, ushort rltId = 18003, int testCount = 3, ClientOption tcpOption = null,
            ClientOption kcpOption = null)
        {
            SocketAddressManager.Instance.SetServerList(serverList);
            TestCount = testCount;
            SetClientOption(tcpOption, kcpOption);
            SetTestSpeedParams(testCount, reqId, rltId);
        }
        
        public SocketSpeedTestHelper(IPAddressInfo[] serverList, ushort reqId = 18002, ushort rltId = 18003, int testCount = 3, ClientOption tcpOption = null,
            ClientOption kcpOption = null)
        {
            SocketAddressManager.Instance.SetServerList(serverList);
            SetClientOption(tcpOption, kcpOption);
            SetTestSpeedParams(testCount, reqId, rltId);
        }

        public void SetClientOption(ClientOption tcpOption, ClientOption kcpOption)
        {
            TcpOption = tcpOption;
            KcpOption = kcpOption;
        }

        public void SetTestSpeedParams(int testCount, ushort reqId, ushort rltId)
        {
            TestCount = testCount;
            ReqTestSpeedProtoId = reqId;
            RltTestSpeedProtoId = rltId;
            _testSpeedProto = new ReqTestSpeed(){ID = ReqTestSpeedProtoId, IsEnd = false};
            var factory = ProtocolFactory.Instance;
            factory.AddProto<ReqTestSpeed>(ReqTestSpeedProtoId);
            factory.AddProto<RltTestSpeed>(RltTestSpeedProtoId);
        }

        
        /// <summary>
        /// 开始速度测试：
        /// 可以在合适的时机调用（如结算后），并加入限制策略（如帧率高于多少的时候、距离上次多长时间之后等）
        /// </summary>
        public void StartSpeedTest()
        {
            if(SocketAddressManager.Instance.AddressInfos == null || SocketAddressManager.Instance.AddressInfos.Count == 0) return;
            StartPingByIndex(0);
        }

        private void StartPingByIndex(int index)
        {
            _currentHostIndex = index;
            _currentCount = 0;
            var address = SocketAddressManager.Instance.AddressInfos[index];
            var type = (ConnectType) (address.Type);
            if (_client != null)
            {
                _client.DisConnect();
            }
            _client = new NetClient.NetClient();
            
            // 测速不需要自动重连。断线则直接加FailCount
            var option = type == ConnectType.Kcp ? KcpOption : TcpOption;
            if (option == null)
            {
                option = new ClientOptionBuilder(type).Build();
            }
            option.NeedAutoReconnect = false;
            option.NeedAutoReconnectTimeOut = false;
            _client.MessageHandler = this;
            _client.Connect(type, address.Host, address.Port, option);
        }

        public void OnReceiveMessage(IRltProto proto)
        {            
            if (proto.ID == RltTestSpeedProtoId)
            {
                var spentTime = GetTickTime() - _lastSendTime;
                var address = SocketAddressManager.Instance.AddressInfos[_currentHostIndex];
                address.AvgTime = address.AvgTime <= 0 ? spentTime : (spentTime + address.AvgTime) / 2f;
                StartNextPing();
            }
        }

        public void OnReceiveLuaMessage(RltLuaMessage proto)
        {
            
        }

        public void OnSocketException(SocketError error, string message)
        {
            OnConnectionFailed();
        }

        public void OnConnectResult(bool success)
        {
            if (success)
            {
                StartNextPing();
            }
        }

        private void OnConnectionFailed()
        {
            // 连接失败不测速，直接FailCount加1
            SocketAddressManager.Instance.AddressInfos[_currentHostIndex].FailCount++;
            _currentCount = TestCount;
            StartNextPing();
        }

        private long GetTickTime()
        {
            var time = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
            return time;
        }

        private void StartNextPing()
        {
            _currentCount++;
            if (_currentCount > TestCount)
            {
                _currentCount = 0;
                _currentHostIndex++;
                if (_currentHostIndex >= SocketAddressManager.Instance.AddressInfos.Count)
                {
                    // 结束
                    SocketAddressManager.Instance.SaveToDisk();
                    _client?.DisConnect();
                    _client = null;
                }
                else
                {
                    // 开始下一个地址
                    StartPingByIndex(_currentHostIndex);
                }
            }
            else
            {
                _testSpeedProto.IsEnd = _currentCount == TestCount;
                _client.SendMessage(_testSpeedProto);
                _lastSendTime = GetTickTime();
            }
        }
    }   
}
