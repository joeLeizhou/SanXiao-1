using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Fsm;

public class GameStateInit :StateBase
{
    public override void StateEnter(params object[] objs)
    {
        base.StateEnter(objs);
        GameLauncher.Instance._stateMachine.ChangeState<GameStateLua>();
    }

    public override void StateEnd(float currentStateTime)
    {
        base.StateEnd(currentStateTime);
    }

    public override void StateFixedUpdate(float currentStateTime)
    {
        base.StateFixedUpdate(currentStateTime);
    }
    
    public override void StateUpdate(float currentStateTime)
    {
        base.StateUpdate(currentStateTime);
    }
}
