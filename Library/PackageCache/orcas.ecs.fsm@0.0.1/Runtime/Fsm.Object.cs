using Orcas.Ecs.Fsm.Interface;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Orcas.Ecs.Fsm
{
    public sealed partial class Fsm : IFsm
    {
        private Entity _self;
        private EntityManager _entityManager;
        private string _binder;
        private float _saveDeltaTime = 0;

        /// <summary>
        /// 获取Fsm实体
        /// </summary>
        /// <returns></returns>
        public Entity GetEntity()
        {
            return _self;
        }
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        /// <param name="systemIndex">在systembase内部设置状态的话需要传入system的index值</param>
        /// <typeparam name="T"></typeparam>
        public void SetState<T>(T state, uint systemIndex = uint.MaxValue) where T : struct, IFsmState
        {
            state.StateEnterTime = Time.time + _saveDeltaTime;
            state.DirtyFlag = _gameLogicLooper.FrameCount + 1;
            state.DirtySystemIndex = systemIndex;
            if (_entityManager.HasComponent<T>(_self))
            {
                _entityManager.SetComponentData(_self, state);
            }
            else
            {
                _entityManager.AddComponentData(_self, state);
            }
        }
        /// <summary>
        /// 设置tag
        /// </summary>
        /// <param name="tag"></param>
        /// <typeparam name="T"></typeparam>
        public void SetTag<T>(T tag) where T : struct, IComponentData
        {
            if (_entityManager.HasComponent<T>(_self))
            {
                _entityManager.SetComponentData(_self, tag);
            }
            else
            {
                _entityManager.AddComponentData(_self, tag);
            }
        }
        /// <summary>
        /// 判断是否包含tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasTag<T>() where T : struct, IComponentData
        {
            return _entityManager.HasComponent<T>(_self);
        }
        /// <summary>
        /// 设置sharedcomponentdata
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void SetSharedComp<T>(T t) where T : struct, ISharedComponentData
        {
            if (_entityManager.HasComponent<T>(_self))
            {
                _entityManager.SetSharedComponentData(_self, t);
            }
            else
            {
                _entityManager.AddSharedComponentData(_self, t);
            }
        }
        
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetState<T>() where T : struct, IFsmState
        {
            return _entityManager.GetComponentData<T>(_self);
        }

        /// <summary>
        /// 获取状态持续时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public float GetStateTime<T>() where T : struct, IFsmState
        {
            return Time.time + _saveDeltaTime - _entityManager.GetComponentData<T>(_self).StateEnterTime;
        }

        /// <summary>
        /// 获取状态持续时间
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float GetStateTime(in IFsmState t)
        {
            return Time.time + _saveDeltaTime - t.StateEnterTime;
        }

        /// <summary>
        /// 判断数据是否被修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal bool CheckChanged<T>(uint systemIndex) where T : struct, IFsmState
        {
            var state = GetState<T>();
            if (state.DirtyFlag < _gameLogicLooper.FrameCount) return false;
            if (state.DirtyFlag > _gameLogicLooper.FrameCount) return true;
            if (state.DirtySystemIndex > systemIndex) return true;
            return false;
        }

        public IFsm GetCopy()
        {
            var copy = Create(_entityManager.World.Name, _binder);
            var input = new NativeArray<Entity>(1, Allocator.Temp);
            input[0] = _self;
            var output = new NativeArray<Entity>(1, Allocator.Temp);
            _entityManager.CopyEntities(input, output);
            copy._self = output[0];
            input.Dispose();
            output.Dispose();
            return copy;
        }

        public void SaveToLocal(string id)
        {
        }

        public void LoadFromLocal(string id)
        {
        }
    }
}
