using Orcas.Game.Common;
using Orcas.Networking;
using System;

namespace Orcas.Game
{
    public interface ILoginHandler
    {
        IClient Client { get; set; }
        void Login(LoginType loginType, string accountID);
        void SetLoginListener(Action<RltLogin> callback);
        void SetBridge(LoginBridgeBase loginBridgeBase);
        void OnLogined(RltLogin rltLogin);
    }
}
