using System.Collections.Generic;
using Orcas.Core;
using SanXiao.Core;

namespace CSharp.Core.Manager
{
    public class FsmManager : IManager
    {
        private List<IStateMachine> _stateMachines;
        
        public StateMachine<T> CreateStateMachine<T>(T target)
        {
            var ret = new StateMachine<T>(target);
            _stateMachines.Add(ret);
            return ret;
        }

        internal void RemoveStateMachine(IStateMachine stateMachine)
        {
            _stateMachines.Remove(stateMachine);
        }
        
        public void Init()
        {
            _stateMachines = new List<IStateMachine>();
        }

        public void Update(uint currentFrameCount)
        {
            for (var i = 0; i < _stateMachines.Count; i++)
            {
                _stateMachines[i].Update();
            }

            for (var i = 0; i < _stateMachines.Count; i++)
            {
                _stateMachines[i].UpdateEndOfFrame();
            }
        }

        public void OnPause()
        {
        }

        public void OnResume()
        {
        }

        public void OnDestroy()
        {
        }
    }
}