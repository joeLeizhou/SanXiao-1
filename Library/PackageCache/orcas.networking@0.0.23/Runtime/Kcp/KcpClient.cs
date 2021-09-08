using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using base_kcp;
using DotNetty.Buffers;
using dotNetty_kcp;
using fec;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Networking.Kcp
{
    public class KcpClient : ClientBase, KcpListener
    {
        private Ukcp _ukcp;
        private string _ip;
        private int _port;
        private ByteBuffer _byteBuffer;
        private dotNetty_kcp.KcpClient _kcpClient;
        private Thread _connectThread;
        
        private int _bufferLen = 65536;
        private ClientOption _option;
        public ClientOption Option {
            get
            {
                return _option;
            }
            set
            {
                _option = value;
                if (_option.BufferSize != _bufferLen)
                {
                    _bufferLen = _option.BufferSize;
                    _byteBuffer = new ByteBuffer(_bufferLen);
                }
            }
        }
        
        public KcpClient(string address, int port)
        {
            NetworkUpdate.Instance.Init();
            NetworkState = NetworkState.DISCONNECTED;
            this._ip = address;
            this._port = port;
            _byteBuffer = new ByteBuffer(_bufferLen);
            Option = new ClientOption();
        }
        
        public override void SendMessage(IReqProto message)
        {
            var bytes = Channel.ClientToServer(message);
            var byteBuf = Unpooled.WrappedBuffer(bytes);
            _ukcp.write(byteBuf);
            byteBuf.Release();
        }

        public override Task<bool> Connect()
        {
            if (!CheckClientEventHandler())
            {
                handleConnectFailed("CheckClientEventHandler Error");
                return Task.FromResult(false);
            }
            
            try
            {
                NetworkState = NetworkState.CONNECTING;
                return Task.Run(async () =>
                {
                    try
                    {
                        DoConnect();
                        while (NetworkState == NetworkState.CONNECTING)
                        {
                            await Task.Delay(20);
                        }
                        return NetworkState == NetworkState.CONNECTED;
                    }
                    catch (Exception e)
                    {
                        handleConnectFailed(e.Message);
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                handleConnectFailed(e.Message);
            }
            return Task.FromResult(false);
        }

        public override void DisConnect()
        {
            NetworkState = NetworkState.DISCONNECTED;
            WaitToClose();
        }

        /// <summary>
        /// 服务器需求：Kcp最后可以给服务器发主动离开房间的协议。确保发出去再断开
        /// </summary>
        private async void WaitToClose()
        {
            while (IsSendListEmpty() == false)
            {
                await Task.Delay(20);
            }
            _ukcp?.close();
            _ukcp = null;
            // 别加stop，会崩溃
            // _kcpClient?.stop();
            _kcpClient = null;
        }

        public void onConnected(Ukcp ukcp)
        {
            NetworkState = NetworkState.CONNECTED;
            _ukcp = ukcp;
        }

        public void handleReceive(IByteBuffer rb, Ukcp ukcp)
        {
            int len = rb.ReadableBytes;
            var bytes = new byte[len];
            rb.GetBytes(rb.ReaderIndex, bytes);
            _byteBuffer.WriteBytes(bytes, len);
            Channel.ServerToClient(_byteBuffer);
        }

        public void handleConnectFailed(string ex)
        {
            NetworkState = NetworkState.DISCONNECTED;
            ClientEventHandler?.OnSocketException(SocketError.ConnectFailedError, ex);
        }

        public void handleException(Exception ex, Ukcp ukcp)
        {
            NetworkState = NetworkState.DISCONNECTED;
            ClientEventHandler?.OnSocketException(SocketError.DisConnectedError, ex.Message);
        }

        public void handleClose(Ukcp ukcp)
        {
            NetworkState = NetworkState.DISCONNECTED;
        }
        
        private bool CheckClientEventHandler()
        {
            return ClientEventHandler == null ? false : true;
        }
        
        
        private void DoConnect()
        {
        
            ChannelConfig channelConfig = new ChannelConfig();
            channelConfig.initNodelay(Option.Nodelay, Option.HeartBeatInterval, Option.Resend, Option.Nc);
            channelConfig.Sndwnd = Option.Sndwnd;
            channelConfig.Rcvwnd = Option.Rcvwnd;
            channelConfig.Mtu = Option.Mtu;

            channelConfig.Crc32Check = Option.Crc32Check;
            channelConfig.AckNoDelay = Option.AckNoDelay;
            channelConfig.Conv = Option.Conv;
            channelConfig.MinRto = Option.MinRto;
            channelConfig.Nodelay = Option.Nodelay;
            channelConfig.TimeoutMillis = Option.TimeOut * 1000;

            _kcpClient = new dotNetty_kcp.KcpClient();
            _kcpClient.init(channelConfig);


            if (IsHostIsAnIPAddress(_ip))
            {
                EndPoint remoteAddress = new IPEndPoint(IPAddress.Parse(_ip), _port);
                _ukcp = _kcpClient.connect(remoteAddress, channelConfig, this);
            }
            else
            {
                try
                {
                    var remoteAddress = GetIPEndPointFromHostName(_ip, _port, false);
                    _ukcp = _kcpClient.connect(remoteAddress, channelConfig, this);
                }
                catch (Exception e)
                {
                    handleException(e, _ukcp);
                }
            }
        }
        
        /// <summary>
        /// Returns true if the Uri's host is a valid IPv4 or IPv6 address.
        /// </summary>
        public static bool IsHostIsAnIPAddress(string host)
        {
            if (string.IsNullOrEmpty(host))
                return false;
            return IsIpV4AddressValid(host) || IsIpV6AddressValid(host);
        }

        // Original idea from: https://www.code4copy.com/csharp/c-validate-ip-address-string/
        // Working regex: https://www.regular-expressions.info/ip.html
        private static readonly System.Text.RegularExpressions.Regex validIpV4AddressRegex = new System.Text.RegularExpressions.Regex("\\b(?:\\d{1,3}\\.){3}\\d{1,3}\\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        /// <summary>
        /// Validates an IPv4 address.
        /// </summary>
        public static bool IsIpV4AddressValid(string address)
        {
            if (!string.IsNullOrEmpty(address))
                return validIpV4AddressRegex.IsMatch(address.Trim());

            return false;
        }

        /// <summary>
        /// Validates an IPv6 address.
        /// </summary>
        public static bool IsIpV6AddressValid(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                System.Net.IPAddress ip;
                if (System.Net.IPAddress.TryParse(address, out ip))
                    return ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }
            return false;
        }
        
        public static IPEndPoint GetIPEndPointFromHostName(string hostName, int port, bool throwIfMoreThanOneIP)
        {
            var addresses = System.Net.Dns.GetHostAddresses(hostName);
            if (addresses.Length == 0)
            {
                throw new ArgumentException(
                    "Unable to retrieve address from specified host name.", 
                    "hostName"
                );
            }
            else if (throwIfMoreThanOneIP && addresses.Length > 1)
            {
                throw new ArgumentException(
                    "There is more that one IP address to the specified host.", 
                    "hostName"
                );
            }
            return new IPEndPoint(addresses[0], port); // Port gets validated here.
        }
        
        public bool IsSendListEmpty()
        {
            return _ukcp.WriteProcessing.Get() == false;
        }
    }
}


