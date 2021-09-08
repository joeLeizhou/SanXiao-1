using System;
using System.Threading.Tasks;
using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;
using Orcas.Networking.Kcp;
using Orcas.Networking.NetClient;
using Orcas.Networking.Tcp;
using UnityEngine;

namespace Orcas.Game.Multiplayer
{
    public abstract partial class Room : IRoom
    {
        public int PlayerCount { get; private set; }
        public long RoomID { get; private set; }
        public int CheckCode { get; private set; }

        private bool _connected;

        public virtual void SendMessage(IReqProto message)
        {
#if TEST
            Debug.Log("send room proto " + message.ID);
#endif
            _client.SendMessage(message);
        }

        protected virtual ReqQuitGame GetQuitGameProto()
        {
            return new DefaultReqQuitGame()
            {
                RoomID = RoomID
            };
        }

        protected virtual ReqStartGame GetStartGameProto()
        {
            return new DefaultReqStartGame();
        }

        protected virtual ReqReconnect GetReconnectRroto()
        {
            return new DefaultReqReconnect()
            {
                UserId = MultiPlayer.MyServerId,
                RoomID = RoomID,
                CheckCode = CheckCode
            };
        }

        protected virtual ReqHeartBeat GetHeartBeatProto()
        {
            return new DefaultReqHeartBeat();
        }

        protected virtual ReqEnterRoom GetEnterRoomProto()
        {
            return new DefaultReqEnterRoom()
            {
                PlayerID = MultiPlayer.MyServerId,
                RoomID = RoomID,
                CheckCode = CheckCode
            };
        }

        protected virtual ReqRenterGame GetRenterGameProto()
        {
            return new DefaultReqRenterGame();
        }

        protected virtual void AddProtos()
        {
            var factory = ProtocolFactory.Instance;

            factory.AddProto<DefaultReqEnterRoom>(MultiPlayerProtoId.ReqEnterRoom, false);
            factory.AddProto<DefaultRltPlayerState>(MultiPlayerProtoId.RltPlayerState, false);
            factory.AddProto<DefaultReqStartGame>(MultiPlayerProtoId.ReqStartGame, false);
            factory.AddProto<DefaultRltRoomState>(MultiPlayerProtoId.RltRoomState, false);
            factory.AddProto<DefaultRltRoundStart>(MultiPlayerProtoId.RltRoundStart, false);
            factory.AddProto<DefaultReqReconnect>(MultiPlayerProtoId.ReqReconnect, false);
            factory.AddProto<DefaultRltReconnect>(MultiPlayerProtoId.RltReconnect, false);
            factory.AddProto<DefaultReqQuitGame>(MultiPlayerProtoId.ReqQuitGame, false);
            factory.AddProto<DefaultReqRenterGame>(MultiPlayerProtoId.ReqRenterGame, false);
            factory.AddProto<DefaultRltRenterGame>(MultiPlayerProtoId.RltRenterGame, false);
        }

        public async void EnterGame(RltMatchInfo matchResult)
        {
            AddProtos();
            _client = new NetClient();
            var clientOption = new ClientOptionBuilder(matchResult.ConnectType)
                .SetAutoReconnectParams(true, 3, true, (int)ReconnectTimeOut)
                .SetHeartBeatProtocols(MultiPlayerProtoId.ReqHeartBeat, MultiPlayerProtoId.RltHeartBeat)
                .Build();
            _client.Connect(matchResult.ConnectType, matchResult.Host, matchResult.Port, clientOption);

            GameSystemBase.SetServer(_client);
            _client.MessageHandler = this;
            var timer = new Timer();
            timer.Start();
            _connected = false;
            while (_connected == false)
            {
                await Task.Delay(20);
                if (timer.RealTime > EnterGameTimeOut)
                {
                    OnGameOver(GameStateReason.ClientCantJoinRoom);
                    break;
                }
            }

            //切换房间状态
            ChangeRoomState(RoomState.WaitForPlayerJoinRoom, GameStateReason.None);

            RoomID = matchResult.RoomID;
            CheckCode = matchResult.CheckCode;

            SendMessage(GetEnterRoomProto());

            // TODO : AddPlayer
            //PlayerCount = matchResult.Players.Length;
            //for (int i = 0; i < PlayerCount; i++)
            //{
            //    AddPlayer(matchResult.Players[i]);
            //}
            var playerInfo = new PlayerNetInfo()
            {
                Id = MultiPlayer.MyServerId,
                RoomState = PlayerRoomState.Joining,
                NetState = PlayerNetState.Connected
            };
            AddPlayer(playerInfo);
            playerInfo = new PlayerNetInfo()
            {
                Id = matchResult.OpponentID,
                RoomState = PlayerRoomState.Joining,
                NetState = PlayerNetState.Connected
            };
            AddPlayer(playerInfo);
        }

        public void SendStartGame()
        {
            SendMessage(GetStartGameProto());
        }
        public void SendQuitGame()
        {
            SendMessage(GetQuitGameProto());
        }
        public void SendRenterGame()
        {
            SendMessage(GetRenterGameProto());
        }
    }
}