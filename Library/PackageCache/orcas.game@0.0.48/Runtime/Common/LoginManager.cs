using Orcas.Core;
using Orcas.Ecs.Fsm.Interface;
using Orcas.Networking;
using System;
using UnityEngine;

namespace Orcas.Game.Common
{
    public class LoginManager : IManager, ILoginHandler
    {
        public IFsm Fsm;
        public IClient Client { get; set; }
        public const string FsmName = "Login";
        public const string WorldName = "Default";
        private LoginBridgeBase _loginBridgeBase;
        private Action<RltLogin> _onLoginListener;

        public void Login(LoginType loginType, string accountID)
        {
            Client.SendMessage(_loginBridgeBase.GetReqLoginProto(loginType, accountID));
        }

        public void SetLoginListener(Action<RltLogin> listener)
        {
            _onLoginListener = listener;
        }

        public void SetBridge(LoginBridgeBase loginBridgeBase)
        {
            _loginBridgeBase = loginBridgeBase;
        }

        public void OnLogined(RltLogin rltLogin)
        {
            _loginBridgeBase.OnLogin(rltLogin);
            _onLoginListener?.Invoke(rltLogin);
        }

        public void Init()
        {
            Fsm = Orcas.Ecs.Fsm.Fsm.Create(WorldName, FsmName);
            _loginBridgeBase = new LoginBridgeBase();
        }

        public void Update(uint currentFrameCount)
        {
        }

        public void OnPause()
        {
        }

        public void OnResume()
        {
        }

        public void OnDestroy()
        {
        }
    }
}