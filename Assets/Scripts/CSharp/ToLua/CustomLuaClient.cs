using LuaInterface;
using UnityEngine;

public class CustomLuaClient : LuaClient
{
	protected override void Awake()
	{
		BindCustom();

		base.Awake();
	}
	public static void BindCustom()
	{
		LuaBinder.BindAction = LuaBinderCustom.Bind;
		LuaBinder.BindBaseAction = LuaBinderCustom.BindBase;

		DelegateFactory.RegisterAction = DelegateFactoryCustom.Register;
	}
	protected override LuaFileUtils InitLoader()
	{
		LuaResLoaderUtils.LuaInitFilesName = "LuaInitFiles";
		return new LuaResLoaderUtils();
	}
}
