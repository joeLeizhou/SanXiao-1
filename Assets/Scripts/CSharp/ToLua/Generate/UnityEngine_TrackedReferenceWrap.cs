﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;
using DelegateCreate = LuaInterface.DelegateFactory.DelegateCreate;

public class UnityEngine_TrackedReferenceWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(UnityEngine.TrackedReference), typeof(System.Object));
		L.RegFunction("Equals", Equals);
		L.RegFunction("GetHashCode", GetHashCode);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.TrackedReference arg0 = (UnityEngine.TrackedReference)ToLua.ToObject(L, 1);
			UnityEngine.TrackedReference arg1 = (UnityEngine.TrackedReference)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Equals(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.TrackedReference obj = (UnityEngine.TrackedReference)ToLua.CheckObject<UnityEngine.TrackedReference>(L, 1);
			object arg0 = ToLua.ToVarObject(L, 2);
			bool o = obj != null ? obj.Equals(arg0) : arg0 == null;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetHashCode(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.TrackedReference obj = (UnityEngine.TrackedReference)ToLua.CheckObject<UnityEngine.TrackedReference>(L, 1);
			int o = obj.GetHashCode();
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
