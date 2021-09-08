using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace Orcas.Lua.Core
{
    public class CLanguageManager {
        /// <summary>
        /// Lua函数
        /// </summary>
        public static LuaFunction GetStringByKeyLuaFunc = null;

        /// <summary>
        /// 得到文字
        /// </summary>
        [NoToLua]
        public static string GetStringByKey(string key, params object[] args)
        {
            string getStr = "";
            if (GetStringByKeyLuaFunc != null)
            {
                GetStringByKeyLuaFunc.BeginPCall();
                GetStringByKeyLuaFunc.Push(key);
                GetStringByKeyLuaFunc.PushArgs(args);
                GetStringByKeyLuaFunc.PCall();
                getStr = GetStringByKeyLuaFunc.CheckString();
                GetStringByKeyLuaFunc.EndPCall();
            }
            return getStr;
        }
    }
}

