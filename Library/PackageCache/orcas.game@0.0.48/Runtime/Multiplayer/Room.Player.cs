using System.Collections.Generic;
using Orcas.Ecs.Fsm;
using Orcas.Ecs.Fsm.Interface;
using Orcas.Game.Multiplayer.Proto;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Orcas.Game.Multiplayer
{
    public abstract partial class Room : IRoom
    {
        private Dictionary<int, IFsm> _playerFsms = new Dictionary<int, IFsm>();

        private void AddPlayer(PlayerNetInfo playerBaseInfo)
        {
            if (_playerFsms.ContainsKey(playerBaseInfo.Id) == false)
            {
                _playerFsms.Add(playerBaseInfo.Id, CreatePlayer(playerBaseInfo));
            }
        }

        private IFsm CreatePlayer(PlayerNetInfo playerBaseInfo)
        {
            var fsm = Ecs.Fsm.Fsm.Create(WorldName, string.Format(PlayerFsmName, playerBaseInfo.Id));
            if (playerBaseInfo.Id == MultiPlayer.MyServerId)
            {
                fsm.SetTag(new MeComponent());
            }

            fsm.SetState(new PlayerComponent()
            {
                RoomState = playerBaseInfo.RoomState,
                NetState = playerBaseInfo.NetState
            });
            fsm.SetState(new ServerObjectComponent()
            {
                ServerId = playerBaseInfo.Id
            });
            //playerBaseInfo.TeamIndex = Mathf.Clamp(playerBaseInfo.TeamIndex, 0, TeamSharedComponent.Teams.Length);
            //fsm.SetSharedComp(TeamSharedComponent.Teams[playerBaseInfo.TeamIndex]);
            return fsm;
        }


        public IFsm GetPlayerFsm(int playerId)
        {
            if (_playerFsms.ContainsKey(playerId))
            {
                return _playerFsms[playerId];
            }

            return null;
        }

        public void SetPlayerState<T>(int playerId, T t) where T : struct, IFsmState
        {
            GetPlayerFsm(playerId).SetState(t);
        }
        
        public T GetPlayerState<T>(int playerId) where T : struct, IFsmState
        {
            return GetPlayerFsm(playerId).GetState<T>();
        }

        public NativeArray<ServerObjectComponent> GetTeam(int index, Allocator allocator)
        {
            var EntityManager = Ecs.Fsm.Fsm.GetEntityManager(WorldName);
            var query = EntityManager.CreateEntityQuery(typeof(TeamSharedComponent), ComponentType.ReadOnly<ServerObjectComponent>());
            query.SetSharedComponentFilter(TeamSharedComponent.Teams[index]);
            return query.ToComponentDataArray<ServerObjectComponent>(allocator);
        }
    }
}