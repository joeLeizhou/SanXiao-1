// using System.Collections;
// using System.Collections.Generic;
// using Unity.Entities;
// using UnityEngine;
// using UnityEngine.LowLevel;
// using Orcas.Game.Multiplayer;
// using System;
// using Orcas.Game.Multiplayer.Proto;
// using Orcas.Core.Tools;
//
// namespace Orcas.Game
// {
//     public class NetWorldManager : IManager
//     {
//
//         public World World;
//         private InitializationSystemGroup _initializationSystemGroup;
//         private SimulationSystemGroup _simulationSystemGroup;
//         private PresentationSystemGroup _presentationSystemGroup;
//
//         private Dictionary<ComponentSystemBase, bool> _systemEnable;
//         
//         public int MePlayerID { get; private set; }
//         public Entity MeEntity { get; private set; }
//         public static bool EnableUpdate { get; private set; }
//         public static float DeltaTime
//         {
//             get
//             {
//                 return Time.fixedDeltaTime;
//             }
//         }
//
//         #region world life circle
//         /// <summary>
//         /// init the world, call on startup
//         /// </summary>
//         public void Init(string worldName)
//         {
//             _systemEnable = new Dictionary<ComponentSystemBase, bool>();
//
//             World = new World(worldName);
//             _initializationSystemGroup = World.CreateSystem<InitializationSystemGroup>();
//             _simulationSystemGroup = World.CreateSystem<SimulationSystemGroup>();
//             _presentationSystemGroup = World.CreateSystem<PresentationSystemGroup>();
//
//             CreateOrAddSystem<BeginInitializationEntityCommandBufferSystem>(1);
//             CreateOrAddSystem<EndInitializationEntityCommandBufferSystem>(1);
//             CreateOrAddSystem<BeginSimulationEntityCommandBufferSystem>(2);
//             CreateOrAddSystem<EndSimulationEntityCommandBufferSystem>(2);
//             CreateOrAddSystem<BeginPresentationEntityCommandBufferSystem>(3);
//
//             var sceneSystem = CreateOrAddSystem<EnterGameSceneSystem>(2);
//
//             sceneSystem.SceneHelper = null; // TODO: SceneManager Implement
//
//             ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World);
//
//         }
//         public T CreateOrAddSystem<T>(int option) where T : ComponentSystemBase
//         {
//             var system = World.GetExistingSystem<T>();
//             if (system != null) return system;
//             system = World.GetOrCreateSystem<T>();
//             if (_systemEnable.ContainsKey(system) == false)
//                 _systemEnable.Add(system, true);
//
//             switch (option)
//             {
//                 case 1:
//                     _initializationSystemGroup.AddSystemToUpdateList(system);
//                     _initializationSystemGroup.SortSystems();
//                     break;
//                 case 2:
//                     _simulationSystemGroup.AddSystemToUpdateList(system);
//                     _simulationSystemGroup.SortSystems();
//                     break;
//                 case 3:
//                     _presentationSystemGroup.AddSystemToUpdateList(system);
//                     _presentationSystemGroup.SortSystems();
//                     break;
//             }
//
//             return system;
//         }
//
//         public void EnableSystem<T>(bool enable) where T : ComponentSystemBase
//         {
//             var system = World.GetExistingSystem<T>();
//             if (system != null) system.Enabled = enable;
//             _systemEnable[system] = enable;
//         }
//         public void TestGame()
//         {
//             var e = World.EntityManager.CreateEntity();
//             World.EntityManager.AddComponentData(e, new PlayerComponent() { });
//             World.EntityManager.AddComponentData(e, new MeComponent());
//         }
//
//         private void Pause(bool pause, Entity exceptEntity = default)
//         {
//             EnableUpdate = !pause;
//
//             _initializationSystemGroup.Enabled = !pause;
//             _simulationSystemGroup.Enabled = !pause;
//             _presentationSystemGroup.Enabled = !pause;
//         }
//
//
//         private void DestroySelf()
//         {
//             if (World != null)
//             {
//                 var entityManager = World.EntityManager;
//                 var entities = entityManager.GetAllEntities();
//                 foreach (var e in entities)
//                     entityManager.DestroyEntity(e);
//                 entities.Dispose();
//                 World.QuitUpdate = true;
//             }
//         }
//
//         public void Init()
//         {
//         }
//
//         public void Update()
//         {
//         }
//
//         public void OnPause()
//         {
//             Pause(true);
//         }
//
//         public void OnResume()
//         {
//             Pause(false);
//         }
//
//         void IManager.OnDestroy()
//         {
//             OnDestroy();
//         }
//
//         private void OnDestroy()
//         {
//             DestroySelf();
//             PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
//             World?.Dispose();
//         }
//         #endregion
//         #region room entity
//         public Entity CreateRoom(long roomId, int stage, int scene, int round = 0, int roundTeam = 0)
//         {
//             var entityManager = World.EntityManager;
//             var e = entityManager.CreateEntity();
//             entityManager.AddComponentData(e, new RoomStateComponent()
//             {
//                 State = RoomState.WaitForSelfEnterGame
//             });
//             entityManager.AddComponentData(e, new GameSceneComponent()
//             {
//                 Stage = stage,
//                 Scene = scene,
//                 LoadState = SceneLoadState.Idle,
//             });
//             entityManager.AddComponentData(e, new GameRoundComponent()
//             {
//                 CurrentRound = round,
//                 CurrentRoundTeam = roundTeam,
//             });
//             entityManager.AddComponentData(e, new ServerObjectComponent()
//             {
//                 ServerId = roomId,
//             });
//             return e;
//         }
//         #endregion
//
//         #region player entity
//         public Entity CreatePlayer(PlayerBaseInfo playerBaseInfo)
//         {
//             var entityManager = World.EntityManager;
//             var e = entityManager.CreateEntity();
//             if (playerBaseInfo.Id == MePlayerID)
//             {
//                 MeEntity = e;
//                 entityManager.AddComponentData(e, new MeComponent());
//             }
//             entityManager.AddComponentData(e, new PlayerComponent()
//             {
//                 RoomState = playerBaseInfo.RoomState,
//                 NetState = playerBaseInfo.NetState
//             });
//             entityManager.AddComponentData(e, new ServerObjectComponent()
//             {
//                 ServerId = playerBaseInfo.Id
//             });
//             playerBaseInfo.TeamIndex = Mathf.Clamp(playerBaseInfo.TeamIndex, 0, TeamSharedComponent.Teams.Length);
//             entityManager.AddSharedComponentData(e, TeamSharedComponent.Teams[playerBaseInfo.TeamIndex]);
//             return e;
//         }
//
//         public void AddPlayerToRoom(Entity roomEntity, Entity playerEntity, int serverID)
//         {
//             var entityManager = World.EntityManager;
//             var buffer = entityManager.GetBuffer<PlayerBufferComponent>(roomEntity);
//             buffer.Add(new PlayerBufferComponent()
//             {
//                 Player = playerEntity,
//                 ServerId = serverID,
//             });
//         }
//         #endregion
//     }
// }