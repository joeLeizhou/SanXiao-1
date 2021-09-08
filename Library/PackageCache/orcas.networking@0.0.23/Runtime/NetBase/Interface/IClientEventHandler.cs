using System;
namespace Orcas.Networking
{
    public enum SocketError
    {
        ConnectFailedError = 0,
        GetHostAddressError,
        ThreadAbortError,
        ReceiveDataError,
        DisConnectedError,
        WriteChannelError,
        WriteByteError,
        SocketDisposedError,
        UnKnownError,
        HandleResultError,
        CanNotReconnectError,
        HeartBeatTimeOutError
    }

    public interface IClientEventHandler
    {
        void OnReceiveMessage(IRltProto proto);
        void OnReceiveLuaMessage(RltLuaMessage proto);
        void OnSocketException(SocketError error, string message);
        void OnConnectResult(bool success);
    }
}