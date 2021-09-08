local #NAME# = {}
local private = {}
local GameObject = UnityEngine.GameObject
local ShortcutExtensions = DG.Tweening.ShortcutExtensions
local RotateMode = DG.Tweening.RotateMode;
local DOVirtual = DG.Tweening.DOVirtual
local ResourceLoader = Orcas.Resources.ResourceLoader;
local UIDepthControl = Orcas.Lua.Core.UIDepthControl;
local UIDragManager = Orcas.Lua.Core.UIDragManager;
local UIEventManager = Orcas.Lua.Core.UIEventManager;

--[Comment]
--某块基本数据
#NAME#.INIT_DATA = {
    #INIT_DATA#
}

--[Comment]
--当前是否可用
#NAME#.IsEnabled = true
--[Comment]
--自己的枚举值
#NAME#.SelfEnum = UIEnum.UIComponetEnum.#SELF_ENUM#

--[Comment]
--第一次显示界面时调用，销毁后也会被重新调用
#NAME#.OnAwake = function(parent)
	private.gameObject = ResourcesManager.LoadGameObject(#NAME#.INIT_DATA.PrefabPath)
	#NAME#.gameObject = private.gameObject
    #NAME#.gameObject.transform:SetParent(parent, false);

	private.objID = private.gameObject:GetInstanceID()
	--LuaModuleManager["#NAME#"..#NAME#.objID]=#NAME#

    private.InitComponent()
    private.AddEventListener()
end

--[Comment]
--有指定GameObject生成
#NAME#.OnAwakeWithGameObject = function(gameObject)
	#NAME#.gameObject = gameObject
	private.gameObject = gameObject
	private.objID = private.gameObject:GetInstanceID()
	--LuaModuleManager["#NAME#"..#NAME#.objID]=#NAME#

    private.InitComponent()
    private.AddEventListener()
end

--[Comment]
--初始化组件
private.InitComponent = function()
#InitComponent#
end

--[Comment]
--在此添加监听，在初始化时调用
private.AddEventListener = function()
#AddEventListener#
end

--[Comment]
--删除监听，默认在销毁时调用
private.RemoveEventListener = function()
#RemoveEventListener#
end

--[Comment]
--由父级调用
#NAME#.Update = function(deltaTime)
    
end

--[Comment]
--由父级调用
#NAME#.UpdateBySecond = function(time)
    
end

--[Comment]
--未被销毁时再次显示时调用
#NAME#.OnShow = function()
    #NAME#.IsEnabled = true
	private.gameObject:SetActive(true);
end

--[Comment]
--隐藏时调用
#NAME#.OnHide = function()
    #NAME#.IsEnabled = false
	private.gameObject:SetActive(false);
end

--[Comment]
--销毁
#NAME#.OnDestroy = function()
	private.RemoveEventListener()
	
    UnityEngine.GameObject.Destroy(private.gameObject)
	#NAME# = nil
	private = nil
end

--在以下添加自定义方法=============================================

return #NAME#
