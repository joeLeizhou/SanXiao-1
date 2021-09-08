using System.Collections.Generic;
using System.IO;
using Orcas.Core;
using Orcas.Ecs.Fsm.Interface;
using Unity.Entities;
using UnityEngine;

namespace Orcas.Ecs.Fsm
{
    public sealed partial class Fsm : IFsm
    {
        private static readonly Dictionary<string, List<Fsm>> FsmDictionary = new Dictionary<string, List<Fsm>>();
        private static Dictionary<string, EntityManager> _worldDictionary = new Dictionary<string, EntityManager>();

        public Fsm()
        {
        }

        /// <summary>
        /// 生成状态机
        /// </summary>
        /// <param name="worldName">world的命名，可以理解为对状态机类型进行一个分类管理</param>
        /// <param name="fsmName">状态机名字，所有状态机仅通过命名进行索引，长度需要少于等于15</param>
        /// <returns></returns>
        public static Fsm Create(string worldName, string fsmName)
        {
            if (fsmName.Length > 15)
            {
                Debug.LogError("状态机名字长度不能超过15");
                return null;
            }
            if (_worldDictionary.ContainsKey(worldName) == false)
            {
                _worldDictionary.Add(worldName, new World(worldName).EntityManager);
            }

            var entityManager = _worldDictionary[worldName];
            var fsm = ReferencePool.GetReference<Fsm>();
            fsm._entityManager = entityManager;
            fsm._self = entityManager.CreateEntity();
            fsm._binder = fsmName;
            entityManager.AddComponentData(fsm._self, new FsmInfoComponent()
            {
                FsmName = fsmName
            });
            if (FsmDictionary.ContainsKey(fsmName) == false)
            {
                FsmDictionary[fsmName] = new List<Fsm>();
            }
            FsmDictionary[fsmName].Add(fsm);
            return fsm;
        }
        /// <summary>
        /// 移除状态机
        /// </summary>
        /// <param name="fsm"></param>
        public static void Remove(Fsm fsm)
        {
            if (fsm == null) return;
            
            fsm._entityManager.DestroyEntity(fsm._self);
            
            var fsms = FsmDictionary[fsm._binder];
            if (fsms == null) return;
            fsms.Remove(fsm);
            ReferencePool.CollectReference(fsm);
        }
        /// <summary>
        /// 按名字获取状态机，如果存在同名状态机则返回第一个状态机
        /// </summary>
        /// <param name="fsmName"></param>
        /// <returns></returns>
        public static Fsm GetFsm(string fsmName)
        {
            if (FsmDictionary.ContainsKey(fsmName) == false || FsmDictionary[fsmName].Count == 0) return null;
            return FsmDictionary[fsmName][0];
        }
        /// <summary>
        /// 获取同名状态机表
        /// </summary>
        /// <param name="fsmName"></param>
        /// <returns></returns>
        public static List<Fsm> GetFsms(string fsmName)
        {
            if (FsmDictionary.ContainsKey(fsmName) == false) return null;
            return FsmDictionary[fsmName];
        }
        
        /// <summary>
        /// 获取world
        /// </summary>
        /// <param name="worldName"></param>
        /// <returns></returns>
        public static World GetWorld(string worldName)
        {
            if (_worldDictionary.ContainsKey(worldName) == false) return null;
            return _worldDictionary[worldName].World;
        }

        /// <summary>
        /// 保存状态机状态
        /// TODO: 改到工作线程进行Save
        /// </summary>
        /// <param name="worldName"></param>
        public static void Save(string worldName)
        {
            if (_worldDictionary.ContainsKey(worldName) == false)
            {
                Debug.LogErrorFormat("尝试保存一个未定义的状态机world:{0}" + worldName);
                return;
            }

            var entityManager = _worldDictionary[worldName];
            //TODO: 优化性能
            foreach (var fsmkv in FsmDictionary)
            {
                for (int i = 0; i < fsmkv.Value.Count; i++)
                {
                    var fsm = fsmkv.Value[i];
                    if (fsm._entityManager == entityManager)
                    {
                        var info = entityManager.GetComponentData<FsmInfoComponent>(fsm._self);
                        info.SaveTime = Time.time;
                        info.Index = i;
                        entityManager.SetComponentData(fsm._self, info);
                    }
                }
            }
            using (var writer =
                new Unity.Entities.Serialization.StreamBinaryWriter(Application.persistentDataPath + "/" + entityManager.World.Name))
            {
                Unity.Entities.Serialization.SerializeUtility.SerializeWorld(entityManager, writer);
            }
        }

        /// <summary>
        /// 通过world的名字获取entitymanager
        /// </summary>
        /// <param name="worldName"></param>
        /// <returns></returns>
        public static EntityManager GetEntityManager(string worldName)
        {
            if (_worldDictionary.ContainsKey(worldName) == false)
            {
                return default;
            }

            return _worldDictionary[worldName];
        }

        /// <summary>
        /// 销毁状态机world
        /// </summary>
        /// <param name="worldName"></param>
        public static void DestroyWorld(string worldName)
        {
            if (_worldDictionary.ContainsKey(worldName) == false) return;
            var manager = _worldDictionary[worldName];
            //TODO: 优化性能
            foreach (var fsmkv in FsmDictionary)
            {
                for (int i = fsmkv.Value.Count - 1; i >= 0; i--)
                {
                    var fsm = fsmkv.Value[i];
                    if (fsm._entityManager == manager)
                    {
                        fsmkv.Value.RemoveAt(i);
                    }
                }
            }
            manager.World.Dispose();
            _worldDictionary.Remove(worldName);
        }

        /// <summary>
        /// 加载状态机状态
        /// TODO: 改到工作线程进行加载
        /// </summary>
        /// <param name="worldName"></param>
        /// <returns></returns>
        public static World Load(string worldName)
        {
            var file = Application.persistentDataPath + "/" + worldName;
            if (!File.Exists(file))
            {
                return null;
            }

            var reader =
                new Unity.Entities.Serialization.StreamBinaryReader(file);
            DestroyWorld(worldName);
            var world = new World(worldName);
            var manager = world.EntityManager;
            _worldDictionary[worldName] = manager;
            manager.PrepareForDeserialize();
            var transaction = manager.BeginExclusiveEntityTransaction();

            Unity.Entities.Serialization.SerializeUtility.DeserializeWorld(transaction, reader);


            manager.EndExclusiveEntityTransaction();
            reader.Dispose();

            var entities = manager.GetAllEntities();

            foreach (var entity in entities)
            {
                if (!manager.HasComponent<FsmInfoComponent>(entity)) continue;
                var info = manager.GetComponentData<FsmInfoComponent>(entity);
                var fsm = ReferencePool.GetReference<Fsm>();
                fsm._binder = info.FsmName.ToString();
                fsm._self = entity;
                fsm._saveDeltaTime = info.SaveTime - Time.time;
                fsm._entityManager = manager;
                if (FsmDictionary.ContainsKey(fsm._binder) == false)
                {
                    FsmDictionary[fsm._binder] = new List<Fsm>();
                }

                var list = FsmDictionary[fsm._binder];
                list.Insert(Mathf.Min(info.Index, list.Count), fsm);
            }

            entities.Dispose();

            return world;
        }
    }
}
