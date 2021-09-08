using System;

namespace LuaInterface
{
    public static class LuaBinder
    {
        public static Action<LuaState> BindAction;
        public static Action<LuaState> BindBaseAction;
        public static void Bind(LuaState L)
        {
            if (BindAction == null)
            {
                throw new LuaException("Please generate CustomLuaBinder files first!");
            }
            else
            {
                BindAction(L);
            }
        }

        public static void BindBase(LuaState L)
        {
            if (BindBaseAction == null)
            {
                throw new LuaException("Please generate CustomLuaBinder files first!");
            }
            else
            {
                BindBaseAction(L);
            }
        }
    }
}