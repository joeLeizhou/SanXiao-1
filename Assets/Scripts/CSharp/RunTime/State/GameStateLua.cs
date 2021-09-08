using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using UnityEngine;
using Orcas.Fsm;
using SanXiao.Game;


public class GameStateLua : StateBase
{
    public LuaStateManager luaStateManager = null;
    public override void StateEnter(params object[] objs)
    {
        base.StateEnter(objs);
        luaStateManager = GameManager.Instance.GetManager<LuaStateManager>();
        Debug.Log("ssssss");
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
        if(luaStateManager != null && luaStateManager.HasInit == true)
        {
            luaStateManager.TempUpdate();
        }
    }
}