using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Orcas.Networking.Tcp
{
    public class TcpClient : ClientBase
    {
        /*发送消息的时候，会在ProtocolFactory中查找是否存在对应ID的消息，然后发送。发送后会使用闭包接收消息，会查找接收消息的ID值，
        然后在ProtocolFactory中查找是否存在对应ID的消息，有的话会调用方法，在主线程中调用相应的Deal（）函数。
        */
        private System.Net.Sockets.TcpClient _tcpClient = null;
        private string _ip;
        private int _port;
        private NetworkStream _stream = null;
        private ByteBuffer _byteBuffer;
        private byte[] _buffer;
        private Thread _receiveThread = null;

        private int _bufferLen = 65536;
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
                if (_option.BufferSize != _bufferLen)
                {
                    _bufferLen = _option.BufferSize;
                    _buffer = new byte[_bufferLen];
                    _byteBuffer = new ByteBuffer(_bufferLen);
                }
            }
        }

        public TcpClient(string address, int port)
        {
            NetworkUpdate.Instance.Init();
            NetworkState = NetworkState.DISCONNECTED;
            this._ip = address;
            this._port = port;
            _buffer = new byte[_bufferLen];
            _byteBuffer = new ByteBuffer(_bufferLen);
            Option = new ClientOption();
        }

        public override Task<bool> Connect()
        {
            if (!CheckClientEventHandler())
            {
                HandleException(SocketError.ConnectFailedError, "CheckClientEventHandler Error");
                return Task.FromResult(false);
            }

            try
            {
                NetworkState = NetworkState.CONNECTING;
                return Task.Run(async () =>
                {
                    try
                    {
                        var dnsTask = Dns.GetHostAddressesAsync(_ip);
                        await dnsTask;
                        var addresses = dnsTask.Result; 
                        for (int i = 0; i < addresses.Length; i++)
                        {
                            if (addresses[i].AddressFamily == AddressFamily.InterNetwork || addresses[i].AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                var client = new System.Net.Sockets.TcpClient(addresses[i].AddressFamily);
                                var address = addresses[i];
                                DisConnect();
                                _tcpClient = client;
                                // Debug.Log("start connect " + _port);
                                await _tcpClient.ConnectAsync(address, _port);

                                if (_tcpClient.Connected == true)
                                {
                                    NetworkState = NetworkState.CONNECTED;
                                    _stream = _tcpClient.GetStream();
                                    if (_receiveThread == null || _receiveThread.IsAlive == false)
                                    {
                                        _receiveThread = new Thread(ReceiveMessage) { IsBackground = true };
                                        _receiveThread.Start();
                                    }
                                }
                                else
                                {
                                    NetworkState = NetworkState.DISCONNECTED;
                                }
                                return _tcpClient.Connected;
                            }
                        }
                        
                        NetworkState = NetworkState.DISCONNECTED;
                        return false;
                    }
                    catch (Exception e)
                    {
                        NetworkState = NetworkState.DISCONNECTED;
                        HandleException(SocketError.ConnectFailedError, e.Message);
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                HandleException(SocketError.GetHostAddressError, e.Message);
                // DebugHelper.LogError(e.Message + "\n" + e.StackTrace);
            }
            NetworkState = NetworkState.DISCONNECTED;
            return Task.FromResult(false);
        }

        private void HandleException(SocketError error, string message)
        {
            //NetworkUpdate.Instance.AddActionDoInMainThread(() =>
            {
                NetworkState = NetworkState.DISCONNECTED;
                ClientEventHandler?.OnSocketException(error, message);
            }//);
        }

        private bool CheckClientEventHandler()
        {
            return ClientEventHandler == null ? false : true;
        }

        public override void DisConnect()
        {
            if (_tcpClient == null)
                return;
            Debug.Log("CloseSocket " + _port);
            if (_stream != null)
                _stream.Close();
            //if (_receiveThread != null)
            //    _receiveThread.Abort();
            _tcpClient.Close();
            _tcpClient.Dispose();
            _tcpClient = null;
            _stream = null;
            //_receiveThread = null;
            NetworkState = NetworkState.DISCONNECTED;
        }

        private void ReceiveMessage()
        {

            while (_networkState == NetworkState.CONNECTED)
            {
                Thread.Sleep(Option.ThreadSleepTime);

                if (_networkState != NetworkState.CONNECTED)
                    continue;

                try
                {
                    if (_tcpClient == null || _tcpClient.Connected == false)
                        continue;
                    if (_stream.CanRead && _stream.DataAvailable)
                    {
                        int bytes = _stream.Read(_buffer, 0, _buffer.Length);
                        if (bytes > 0)
                        {
                            _byteBuffer.WriteBytes(_buffer, bytes);
                            Channel.ServerToClient(_byteBuffer);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    HandleException(SocketError.ThreadAbortError, "ThreadAbortException");
                }
                catch (Exception e)
                {
                    HandleException(SocketError.ReceiveDataError, e.Message + "\n" + e.StackTrace);
                }
            }
        }

        private SocketError _socketError;
        public override void SendMessage(IReqProto message)
        {
            _socketError = SocketError.DisConnectedError;
            try
            {
                if (_networkState != NetworkState.CONNECTED || message == null || _tcpClient == null || _tcpClient.Connected == false)
                {
                    HandleException(_socketError, "Send message when Tcp not connected " + ",proto:" + message.ID);
                    return;
                }
                _socketError = SocketError.WriteChannelError;

                var bytes = Channel.ClientToServer(message);

                _socketError = SocketError.WriteByteError;

                if (_stream != null)
                {
                    _stream.Write(bytes, 0, bytes.Length);
                    _stream.Flush();
                }
                else
                {
                    UnityEngine.Debug.LogError("stream is null!");
                }
                //Debug.Log("send message " + message.ID);
            }
            catch (Exception e)
            {
                HandleException(_socketError, e.Message + "\n" + e.StackTrace + ",proto:" + message.ID);
            }
        }
    }
}
