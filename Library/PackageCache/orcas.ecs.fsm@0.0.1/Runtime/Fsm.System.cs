using System;
using System.Collections.Generic;
using Orcas.Ecs.Fsm.Interface;
using Orcas.Core;
using UnityEngine.PlayerLoop;

namespace Orcas.Ecs.Fsm
{
    public sealed partial class Fsm : IFsm
    {
        private static GameLogicLooper _gameLogicLooper;
        public static GameLogicLooper GameLogicLooper
        {
            set
            {
                if (_gameLogicLooper != null)
                {
                    _gameLogicLooper.PreUpdateQueue -= Update;
                }

                _gameLogicLooper = value;
                _gameLogicLooper.PreUpdateQueue += Update;
            }
        }

        private static readonly List<FsmSystemBase> Systems = new List<FsmSystemBase>();
        private static Dictionary<Type, FsmSystemBase> _systemDictionary = new Dictionary<Type, FsmSystemBase>();
        
        /// <summary>
        /// 加入system到链表末尾
        /// system执行顺序严格按照加入顺序执行
        /// </summary>
        /// <param name="systemBase"></param>
        public static void AddSystemBase<T>() where T : FsmSystemBase, new()
        {
            var type = typeof(T);
            if (_systemDictionary.ContainsKey(type)) return;
            var systemBase = new T();
            _systemDictionary.Add(type, systemBase);
            systemBase.Index = Systems.Count == 0 ? 64 : Systems[Systems.Count - 1].Index + 64;
            Systems.Add(systemBase);
        }
        /// <summary>
        /// 加入system到某个中间位置
        /// 尽量少用这个方法，因为index算法比较取巧
        /// </summary>
        /// <param name="systemBase"></param>
        public static void AddSystemAfterType<T>(Type type) where T : FsmSystemBase, new()
        {
            var sysType = typeof(T);
            if (_systemDictionary.ContainsKey(sysType)) return;
            for (int i = 0; i < Systems.Count; i++)
            {
                if (Systems[i].GetType() == type)
                {
                    if (i == Systems.Count - 1)
                    {
                        AddSystemBase<T>();
                        return;
                    }
                    else
                    {
                        var systemBase = new T();
                        _systemDictionary.Add(sysType, systemBase);
                        systemBase.Index = (Systems[i].Index + Systems[i + 1].Index) / 2;
                        Systems.Insert(i + 1, systemBase);
                        return;
                    }
                }
            }
            AddSystemBase<T>();
        }
        /// <summary>
        /// 加入system到某个中间位置
        /// 尽量少用这个方法，因为index算法比较取巧
        /// </summary>
        /// <param name="systemBase"></param>
        public static void AddSystemBeforeType<T>(Type type) where T : FsmSystemBase, new()
        {
            var sysType = typeof(T);
            if (_systemDictionary.ContainsKey(sysType)) return;
            for (int i = 0; i < Systems.Count; i++)
            {
                if (Systems[i].GetType() == type)
                {
                    if (i == 0)
                    {
                        var systemBase = new T(); 
                        _systemDictionary.Add(sysType, systemBase);
                        systemBase.Index = Systems[0].Index / 2;
                        Systems.Insert(0, systemBase);
                        return;
                    }
                    else
                    {
                        var systemBase = new T();
                        _systemDictionary.Add(sysType, systemBase);
                        systemBase.Index = (Systems[i].Index + Systems[i - 1].Index) / 2;
                        Systems.Insert(i, systemBase);
                        return;
                    }
                }
            }
            AddSystemBase<T>();
        }
        /// <summary>
        /// 移除System
        /// </summary>
        /// <param name="system"></param>
        public static void RemoveSystem<T>() where T : FsmSystemBase, new()
        {
            var sysType = typeof(T);
            if (_systemDictionary.ContainsKey(sysType) == false) return;
            Systems.Remove(_systemDictionary[sysType]);
            _systemDictionary.Remove(sysType);
        }
        
        /// <summary>
        /// Update
        /// </summary>
        private static void Update()
        {
            //TODO: 避免update中system被删除出错，这里需要优化一些实现方式
            foreach (var system in Systems)
            {
                system.OnUpdate();
            }   
        }
    }
}