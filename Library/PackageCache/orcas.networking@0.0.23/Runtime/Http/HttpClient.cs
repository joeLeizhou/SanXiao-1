using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Orcas.Networking.Http
{
    public class HttpClient : ClientBase
    {
        private string _url;
        private ByteBuffer _byteBuffer;
        public HttpClient(string url)
        {
            NetworkUpdate.Instance.Init();
            NetworkState = NetworkState.DISCONNECTED;
            this._url = url;
            Debug.Log("set url " + url);
            _byteBuffer = new ByteBuffer(65536);
            Option = new ClientOption();
        }

        public override Task<bool> Connect()
        {
            if (!CheckClientEventHandler())
                return Task.FromResult(false);

            try
            {
                NetworkState = NetworkState.CONNECTED;
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                HandleException(SocketError.GetHostAddressError, e.Message);
                // DebugHelper.LogError(e.Message + "\n" + e.StackTrace);
            }

            return Task.FromResult(false);
        }

        private void HandleException(SocketError error, string message)
        {
            //NetworkUpdate.Instance.AddActionDoInMainThread(() =>
            {
                ClientEventHandler.OnSocketException(error, message);
            }//);
        }

        private bool CheckClientEventHandler()
        {
            return ClientEventHandler == null ? false : true;
        }

        public override void DisConnect()
        {

        }

        private void ReceiveMessage()
        {

        }

        private SocketError _socketError;
        public override void SendMessage(IReqProto message)
        {
            _socketError = SocketError.DisConnectedError;
            try
            {
                _socketError = SocketError.WriteChannelError;

                var bytes = Channel.ClientToServer(message);

                _socketError = SocketError.WriteByteError;
                NetworkUpdate.Instance.StartCoroutine(SendBytes(bytes, message.ID));
                Debug.Log("send message " + message.ID);
            }
            catch (Exception e)
            {
                HandleException(_socketError, e.Message + "\n" + e.StackTrace);
            }
        }
        Dictionary<string, string> _headers;
        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
        }

        public IEnumerator SendBytes(byte[] bytes, ushort id)
        {
            var request = new UnityWebRequest(_url, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = Option.TimeOut;
            if (_headers != null)
            {
                foreach (var item in _headers)
                {
                    request.SetRequestHeader(item.Key, item.Value);
                }
            }

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                HandleException(SocketError.ConnectFailedError, request.error + ",proto:" + id.ToString());
            }
            else if (request.isHttpError)
            {
                HandleException(SocketError.HandleResultError, request.error + ",proto:" + id.ToString());
            }
            else
            {
                try
                {
                    var _buffer = request.downloadHandler.data;
                    var len = _buffer.Length;
                    if (len > 0)
                    {
                        _byteBuffer.WriteBytes(_buffer, len);
                        Channel.ServerToClient(_byteBuffer);
                    }
                }
                catch (Exception e)
                {
                    HandleException(SocketError.ReceiveDataError, e.Message + ",proto:" + id.ToString());
                }
            }
        }
    }
}
