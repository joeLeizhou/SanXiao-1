using System;
using System.Runtime.CompilerServices;
using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;
using Orcas.Networking.Tcp;
using UnityEngine;

namespace Orcas.Game.Multiplayer
{
    public abstract partial class Room : IRoom
    {
        private float _lastMessageTime;

        public abstract void OnPlayerJoined(int playerId);

        public abstract void OnPlayerQuitGame(int playerId);

        public abstract void OnPlayerDisconnected(int playerId);

        public abstract void OnGamePause();

        public abstract void OnGameResume();

        public abstract void OnStartGame();

        public abstract void OnRoundStart(RltRoundStart proto);

        public abstract void OnRoomStateChanged(RoomState state);

        public abstract void OnGameOver(GameStateReason reason);

        public abstract void OnReconnected(RltReconnect proto);

        public abstract void OnRenterGamed(RltRenterGame proto);

        public abstract void OnDisconnected(SocketError socketError);

        public void OnConnectResult(bool success)
        {
            _connected = success;
            
            // TODO: 房间状态不知道什么时候需要发重连协议
            // var curStateCom = RoomFsm.GetState<RoomStateComponent>();
            // var curState = curStateCom.State;
            // if (curState == RoomState.Pause)
            // {
            //     _client.SendMessage(GetReconnectRroto());
            // }
        }
        

        private void ChangeRoomState(RoomState nextState, GameStateReason overReason)
        {
            var oldState = RoomFsm.GetState<RoomStateComponent>();
            OnRoomStateChanged(nextState);

            if (oldState.State == nextState) return;

            RoomFsm.SetState(new RoomStateComponent()
            {
                State = nextState
            });
            if (oldState.State != RoomState.Pause && nextState == RoomState.Running)
            {
                OnStartGame();
            }

            if (oldState.State == RoomState.Pause && nextState == RoomState.Running)
            {
                MultiPlayer.OnResume();
                OnGameResume();
            }

            if (oldState.State == RoomState.Running && nextState == RoomState.Pause)
            {
                MultiPlayer.OnPause();
                OnGamePause();
            }

            if (oldState.State != RoomState.GameOver && nextState == RoomState.GameOver)
            {
                OnGameOver(overReason);
            }
        }

        private void ChangePlayerState(RltPlayerState nextInfo)
        {
            var oldState = GetPlayerState<PlayerComponent>(nextInfo.PlayerId);
            SetPlayerState(nextInfo.PlayerId, new PlayerComponent()
            {
                RoomState = nextInfo.RoomState,
                NetState = nextInfo.NetState
            });
            if (oldState.RoomState != PlayerRoomState.JoinedRoom &&
                nextInfo.RoomState == PlayerRoomState.JoinedRoom)
            {
                OnPlayerJoined(nextInfo.PlayerId);
            }

            if (oldState.RoomState != PlayerRoomState.QuitRoom &&
                nextInfo.RoomState == PlayerRoomState.QuitRoom)
            {
                OnPlayerQuitGame(nextInfo.PlayerId);
            }

            if (oldState.NetState != PlayerNetState.Disconnected &&
                nextInfo.NetState == PlayerNetState.Disconnected)
            {
                OnPlayerDisconnected(nextInfo.PlayerId);
            }
        }

        public virtual void OnReceiveMessage(IRltProto proto)
        {
            _lastMessageTime = Time.realtimeSinceStartup;
            switch (proto.ID)
            {
                case MultiPlayerProtoId.RltRoomState:
                    {
                        ChangeRoomState(((RltRoomState)proto).State, ((RltRoomState)proto).Reason);
                        break;
                    }
                case MultiPlayerProtoId.RltPlayerState:
                    {
                        ChangePlayerState((RltPlayerState)proto);
                        break;
                    }
                case MultiPlayerProtoId.RltRoundStart:
                    {
                        var roundInfo = ((RltRoundStart)proto);
                        RoomFsm.SetState(new GameRoundComponent()
                        {
                            CurrentRound = roundInfo.Round,
                            CurrentRoundPlayer = roundInfo.PlayerID
                        });
                        OnRoundStart(roundInfo);
                        break;
                    }
                case MultiPlayerProtoId.RltReconnect:
                    {
                        var tProto = (RltReconnect)proto;
                        ChangeRoomState(tProto.RoomState, tProto.Reason);
                        //foreach (var players in tProto.Players)
                        //{
                        //    ChangePlayerState(players);
                        //}
                        //RoomFsm.SetState(new GameRoundComponent()
                        //{
                        //    CurrentRound = tProto.Round,
                        //    CurrentRoundPlayer = tProto.RoundInfo.RoundTeamIndex
                        //});
                        OnReconnected(tProto);
                        break;
                    }
                case MultiPlayerProtoId.RltRenterGame:
                    {
                        var rProto = (RltRenterGame)proto;
                        OnRenterGamed(rProto);
                        break;
                    }
            }
        }
        public virtual void OnReceiveLuaMessage(RltLuaMessage proto)
        {

        }

        public void OnUpdate()
        {
            
        }


        public virtual void OnSocketException(SocketError error, string message)
        {
            var state = RoomFsm.GetState<RoomStateComponent>();
            Debug.Log("error " + error + ":" + message);

            OnDisconnected(error);
            if (state.State == RoomState.GameOver || state.State == RoomState.WaitForSelfEnterGame)
            {
                return;
            }

            if (error == SocketError.CanNotReconnectError)
            {
                Debug.LogError("can not reconnect");
                ChangeRoomState(RoomState.GameOver, GameStateReason.ClientDisconnect);
            }
        }
    }
}