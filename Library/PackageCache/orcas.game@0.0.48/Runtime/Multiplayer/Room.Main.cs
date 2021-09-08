using System;
using Orcas.Ecs.Fsm.Interface;
using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;
using Orcas.Networking.NetClient;
using Orcas.Networking.Tcp;

namespace Orcas.Game.Multiplayer
{
    public abstract partial class Room : IRoom
    {
        public long Id { get; }
        public IFsm RoomFsm { get; private set; }
        public MultiPlayerManager MultiPlayer { get; private set; }
        private NetClient _client;
        private int _stage, _scene;
        private const float EnterGameTimeOut = 30;
        private const float ReconnectTimeOut = 30;
        private const string WorldName = "Battle";
        private const string RoomFsmName = "RoomState";
        private const string PlayerFsmName = "p{0}";

        protected Room(long id, int stage, int scene)
        {
            Id = id;
            _stage = stage;
            _scene = scene;
            GameSystemBase.SetRoom(this);
            RoomFsm = Ecs.Fsm.Fsm.Create(WorldName, "RoomState");
            RoomFsm.SetState(new RoomStateComponent()
            {
                State = RoomState.WaitForSelfEnterGame
            });
            RoomFsm.SetState(new GameSceneComponent()
            {
                Stage = _stage,
                Scene = _scene,
                LoadState = SceneLoadState.Idle,
            });
            RoomFsm.SetState(new GameRoundComponent()
            {
                CurrentRound = 0,
                CurrentRoundPlayer = 0,
            });
            RoomFsm.SetState(new ServerObjectComponent()
            {
                ServerId = Id,
            });
            Ecs.Fsm.Fsm.AddSystemBase<EnterGameSceneSystem>();
        }

        void IRoom.SetMultiplayer(MultiPlayerManager multiPlayer)
        {
            MultiPlayer = multiPlayer;
        }
        
        public void CloseRoom()
        {
            if (_client != null)
            {
                _client.DisConnect();
                _client.MessageHandler = null;
                _client = null;
            }
            MultiPlayer.SetRoom(null);
            Ecs.Fsm.Fsm.RemoveSystem<EnterGameSceneSystem>();
        }
    }

    public class DefaultRoom : Room
    {
        public override void OnPlayerJoined(int playerId)
        {
        }

        public override void OnPlayerQuitGame(int playerId)
        {
        }

        public override void OnStartGame()
        {
        }

        public override void OnPlayerDisconnected(int playerId)
        {
        }

        public override void OnGamePause()
        {
        }

        public override void OnGameResume()
        {
        }

        public override void OnRoundStart(RltRoundStart proto)
        {
        }

        public override void OnGameOver(GameStateReason reason)
        {
        }

        public override void OnReconnected(RltReconnect proto)
        {
        }

        public override void OnDisconnected(SocketError socketError)
        {
        }

        public override void OnRoomStateChanged(RoomState state)
        {
        }

        public override void OnRenterGamed(RltRenterGame proto)
        {
        }

        public DefaultRoom(long id, int stage, int scene) : base(id, stage, scene)
        {
        }
    }
}