using Orcas.Ecs.Fsm.Interface;

namespace Orcas.Ecs.Fsm
{
    public abstract class FsmSystemBase
    {
        internal uint Index { get; set; }
        protected internal abstract void OnUpdate();

        protected FsmSystemBase()
        {
            
        }
        
        protected bool CheckChanged<T>(Fsm fsm) where T : struct, IFsmState
        {
            return fsm.CheckChanged<T>(Index);
        }

        protected void SetState<T>(IFsm fsm, T state) where T : struct, IFsmState
        {
            fsm.SetState(state, Index == 0 ? uint.MaxValue : Index);
        }
    }
}