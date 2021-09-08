using Orcas.Core;
using Unity.Entities;

namespace Orcas.Ecs.Fsm.Interface
{
    public interface IFsm : IReference
    {
        void SetState<T>(T state, uint systemIndex = uint.MaxValue) where T : struct, IFsmState;
        T GetState<T>() where T : struct, IFsmState;
        float GetStateTime<T>() where T : struct, IFsmState;
        void SetTag<T>(T tag) where T : struct, IComponentData;
        bool HasTag<T>() where T : struct, IComponentData;
        void SetSharedComp<T>(T t) where T : struct, ISharedComponentData;
        float GetStateTime(in IFsmState t);
        IFsm GetCopy();
        void SaveToLocal(string id);
        void LoadFromLocal(string id);
    }
}