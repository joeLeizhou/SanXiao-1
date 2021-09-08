namespace Orcas.Fsm {
    public abstract class StateBase {
        
        public virtual void StateEnter (params object[] objs) {
            
        }

        public virtual void StateUpdate (float currentStateTime){
			
		}

        public virtual void StateFixedUpdate (float currentStateTime){
            
        }

        public virtual void StateEnd (float currentStateTime){
			
		}
    }
}