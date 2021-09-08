UIManager = {}


UIDepthEnum = {
    LAYER_BOTTOM = 0,
    --默认
    LAYER_NORMAL = 1;
    --中层
    LAYER_MIDDLE = 2,
    --顶层
    LAYER_TOP = 3,
    --最高层
    LAYER_HIGHEST = 4,

    LAYER_COVER_ALL = 5,
}

UISortingOrderEnum = {
    [UIDepthEnum.LAYER_BOTTOM] = -300,
    [UIDepthEnum.LAYER_NORMAL] = -200,
    [UIDepthEnum.LAYER_MIDDLE] = -100,
    [UIDepthEnum.LAYER_TOP] = 0,
    [UIDepthEnum.LAYER_HIGHEST] = 100,
    [UIDepthEnum.LAYER_COVER_ALL] = 500,
}

local private = {}
local GameObject = UnityEngine.GameObject
local Time = Time
local DOVirtual = DG.Tweening.DOVirtual;
local frameTimer = 0
private.IntentShowQueue = {}
private.UIStack = {}
private.UIData = {}
private.OnUpdateEvent = nil

UIManager.FINISH_POPUP_WINDOW = "UIManager_FINISH_POPUP_WINDOW"
UIManager.CLICK_BACK_EMPTY = "UIManager_CLICK_BACK_EMPTY"

UIManager.Init = function()
    private.UISystemObj = GameObject.Find("UISystem")
    GameObject.DontDestroyOnLoad(private.UISystemObj)
    private.UICanvasObj_2D = private.UISystemObj.transform:Find("Canvas2D")
    private.UICamera = private.UISystemObj.transform:Find("UICamera").gameObject:GetComponent("Camera")
    private.LayerNode = {}
    private.LayerNode[UIDepthEnum.LAYER_BOTTOM] = private.UICanvasObj_2D.transform:Find("BottomNode")
    private.LayerNode[UIDepthEnum.LAYER_NORMAL] = private.UICanvasObj_2D.transform:Find("NormalNode")
    private.LayerNode[UIDepthEnum.LAYER_MIDDLE] = private.UICanvasObj_2D.transform:Find("MiddleNode")
    private.LayerNode[UIDepthEnum.LAYER_TOP] = private.UICanvasObj_2D.transform:Find("TopNode")
    private.LayerNode[UIDepthEnum.LAYER_HIGHEST] = private.UICanvasObj_2D.transform:Find("HighestNode")
    private.LayerNode[UIDepthEnum.LAYER_COVER_ALL] = private.UICanvasObj_2D.transform:Find("CoverAllNode")

    private.UIData = {}
    private.IntentShowQueue = {}
    private.UIStack = {}
    private.RegisterUpdateEvent();
end

UIManager.HasActiveObjInNode = function (depthEnum)
    local root = private.LayerNode[depthEnum]
    for i = 0, root.transform.childCount - 1 do
        local child = root.transform:GetChild(i)
        if child.gameObject.activeInHierarchy then
            return true
        end
    end
    return false
end

private.RegisterUpdateEvent = function()
    UIManager.OnUpdateEvent = UpdateBeat:CreateListener(private.Update)
    UpdateBeat:AddListener(UIManager.OnUpdateEvent)
end

private.Update = function()
    private.ClickBack();

    if private.UIData == nil then
        return
    end

    for k, v in pairs(private.UIData) do
        if v.active == true and v.needUpdate == true then
            if v.ui.Update ~= nil then
                v.ui.Update(Time.deltaTime)
            end
        end
    end

    private.UpdateBySecond()
end

private.UpdateBySecond = function()
    frameTimer = frameTimer + Time.deltaTime
    local needSecUpdate = frameTimer >= 1
    if needSecUpdate == true then
        for k, v in pairs(private.UIData) do
            if v.active == true and v.needSecondUpdate == true then
                if v.ui.UpdateBySecond ~= nil then
                    v.ui.UpdateBySecond()
                end
            end
        end
        frameTimer = 0;
    end
end

private.ClickBack = function()
    if UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape) then
        private.HideTopPanel()
    end
end

private.HideTopPanel = function()
    local hasNeedBackUI = false
    local stackLength = #private.UIStack
    if stackLength > 0 then
        local topUIName = private.UIStack[stackLength];
        if private.UIData[topUIName] ~= nil and private.UIData[topUIName].ui ~= nil and private.UIData[topUIName].ui.NeedClickBack then
            UIManager.HideUI(topUIName);
            hasNeedBackUI = true;
        end
    end

    if hasNeedBackUI == false then
        MessageEventManager.BroadcastMessage(UI_EVENT_TYPE.FINISH_POPUP_WINDOW)
    end
end

UIManager.CreateComponent = function(uiName, parentNode)
    local component = private.loadUILuaFile(uiName)
    if component.OnAwake then
        component.OnAwake(parentNode.transform)
    end
    return component
end

UIManager.GetInComponent = function(uiName, parentNode)
    local component = private.loadUILuaFile(uiName)
    if component.OnAwakeWithGameObject then
        component.OnAwakeWithGameObject(parentNode.transform)
    end
    return component
end

UIManager.GetInComponent = function(uiName, gameObj)
    local ComponentEntity = private.loadUILuaFile(uiName)
    if ComponentEntity.OnAwakeWithGameObject then
        ComponentEntity.OnAwakeWithGameObject(gameObj)
    end
    return ComponentEntity
end

UIManager.GetUI = function(uiName)
    if private.UIData[uiName] ~= nil then
        return private.UIData[uiName].ui;
    end
    return nil;
end

UIManager.ShowUI = function(uiName)
    local ui;
    if private.UIData[uiName] == nil or private.UIData[uiName].ui == nil then
        ui = private.CreateUIModule(uiName, true)
        private.SetUIDepth(ui)
    else
        ui = private.UIData[uiName].ui;
        private.UIData[uiName].active = true
        private.SetUIPanelActive(ui, true)
        if ui.OnShow then
            ui.OnShow()
        end
    end
    ui.gameObject:SetActive(private.UIData[uiName].active)
    private.UIStack[#private.UIStack + 1] = uiName;
    ui.gameObject.transform:SetSiblingIndex(999)
    MessageEventManager.BroadcastMessage(DATA_TYPE.ON_CLICK_SHOWUI_BUBBLE)
    return ui;
end

private.SetUIPanelActive = function(ui, show)
    ui.IsEnabled = show
    SetObjectActive(ui.gameObject, show)
end

private.CreateUIModule = function(uiName, isModule)
    if uiName == nil then
        error("check luaFileName")
    end
    
    local ui = private.loadUILuaFile(uiName)
    private.UIData[uiName] = {
        ui = ui,
        needUpdate = ui.INIT_DATA.NeedUpdate,
        needSecondUpdate = ui.INIT_DATA.NeedSecondUpdate,
        active = true;
    };
    
    if isModule == true then
        if ui.OnAwake then
            ui.OnAwake()
        end
        
        if ui.OnShow then
            ui.OnShow()
        end
    end
    return ui
end

private.ClearUIModule = function(luaFileName)
    if luaFileName == nil then
        error("check luaFileName")
    end

    private.UIData[luaFileName] = nil
end

private.loadUILuaFile = function(luaFileName)
    local f = _G.loadfile(luaFileName)
    return f()
end

private.SetUIDepth = function(ui)
    local mTrans = ui.gameObject.transform
    local pTrans = ui.gameObject.transform.parent
    if pTrans == nil or pTrans:Equals(nil) then
        mTrans:SetParent(private.LayerNode[ui.INIT_DATA.Depth], false)
        mTrans.localPosition = Vector3.New(mTrans.localPosition.x, mTrans.localPosition.y, 0)
    end
end

private.RemoveFromUIStack = function(uiName)
    for i = #private.UIStack, 1, -1 do
        local curName = private.UIStack[i];
        if uiName == curName then
            table.remove(private.UIStack, i);
            break ;
        end
    end
end

UIManager.HideUI = function(uiName)

    local uiData = private.UIData[uiName]
    if uiData ~= nil and uiData.ui and uiData.ui.IsEnabled == true and uiData.active == true then
        uiData.active = false
        if uiData.ui.OnHide ~= nil then
            if uiData.ui.CloseAnimDuration and uiData.ui.CloseAnimDuration > 0 then
                DOVirtual.DelayedCall(
                        uiData.ui.CloseAnimDuration,
                        function()
                            private.SetUIPanelActive(uiData.ui, false)
                            uiData.ui.OnHide()
                        end
                )
            else
                private.SetUIPanelActive(uiData.ui, false)
                uiData.ui.OnHide()
            end
        end
    end

    private.RemoveFromUIStack(uiName);
    private.IntentShowNextUI(uiName);
end

UIManager.DestroyUI = function(uiName)
    local uiData = private.UIData[uiName]
    if uiData and uiData.ui then
        local ui = uiData.ui;
        if ui.OnDestroy then
            if ui.CloseAnimDuration and ui.CloseAnimDuration > 0 then
                DOVirtual.DelayedCall(
                        ui.CloseAnimDuration,
                        function()
                            ui.OnDestroy()
                        end
                )
            else
                ui.OnDestroy()
            end
        end
        private.ClearUIModule(uiName)
    end

    private.RemoveFromUIStack(uiName);
    private.IntentShowNextUI(uiName);
end

UIManager.DestroyAllUI = function()
    if private.UIData then
        for k, v in pairs(private.UIData) do
            if v.ui and v.ui.OnDestroy then
                v.ui.OnDestroy()
            end
        end
        print("lua destroy all ui finish")
        _G.collectgarbage("collect")
        print("lua gc finish")
    end

    UIManager.ClearStack();
end

private.ShowFunction = function(key, callback)
    private.UIData[key] = callback;
    private.UIStack[#private.UIStack + 1] = key;
end

UIManager.HideFunction = function(key)
    private.RemoveFromUIStack(key);
    private.IntentShowNextUI(key);
end

UIManager.ClearStack = function()
    private.UIStack = {};
    private.UIData = {};
    private.IntentShowQueue = {};
end

UIManager.HideAllUI = function()
    if private.UIData then
        for k, v in pairs(private.UIData) do
            local ui = v.ui;
            if ui ~= nil and ui.OnHide and ui.IsEnabled == true and v.active == true then
                v.active = false
                ui.OnHide()
            end
        end
    end
end

-- 参数：
-- uiName：要展示的UI的名字
-- preCallback：作为数据准备用，在展示UI之前调用；
-- Type: 1.ShowPanel 3.Function
UIManager.IntentToShowUI = function(uiName, preCallback, isFirst)
    private.Enqueue(isFirst, {
        Key = uiName,
        Type = 1,
        Callback = preCallback
    })
end

UIManager.IntentToShowFunc = function(funcKey, preCallback, isFirst)
    private.Enqueue(isFirst, {
        Key = funcKey,
        Type = 2,
        Callback = preCallback
    })
end

private.Enqueue = function(isFirst, data)
    if(isFirst and #private.IntentShowQueue > 1) then
        table.insert(private.IntentShowQueue, 2, data);
    else
        table.insert(private.IntentShowQueue, data);
    end

    if #private.IntentShowQueue == 1 then
        private.IntentShowUI();
    end
end

UIManager.CheckIntentShowQueueContains = function(Key)
    for i = 1, #private.IntentShowQueue do
        local data = private.IntentShowQueue[i];
        if data.Key == Key then
            return true;
        end
    end
    return false;
end

private.IntentShowUI = function()
    if #private.IntentShowQueue > 0 then
        local record = private.IntentShowQueue[1];
        if record.Type == 1 then
            UIManager.ShowUI(record.Key);
        else
            private.ShowFunction(record.Key, record.Callback)
        end

        if record.Callback then
            record.Callback();
        end
    end
end

private.IntentShowNextUI = function(key)
    if #private.IntentShowQueue > 0 then
        -- 移除当前的记录
        local record = private.IntentShowQueue[1];
        if key == record.Key then
            table.remove(private.IntentShowQueue, 1)
            private.IntentShowUI();
        end
    end

    if #private.IntentShowQueue == 0 then
        MessageEventManager.BroadcastMessage(UI_EVENT_TYPE.FINISH_POPUP_WINDOW)
    end
end


UIManager.GetUISystemObj = function()
    return private.UISystemObj
end

UIManager.GetCanvasObj_2D = function()
    return private.UICanvasObj_2D
end

UIManager.GetLayer = function (layerType)
    return private.LayerNode[layerType]
end

UIManager.GetUICamera = function()
    return private.UICamera
end

UIManager.GetUIPos = function(worldPos, recttrans)
    local screenPos = UnityEngine.RectTransformUtility.WorldToScreenPoint(UnityEngine.Camera.main, worldPos)
    if not recttrans then
        recttrans = private.UICanvasObj_2D:GetComponent("RectTransform")
    end
    local s, localpos = UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(recttrans, screenPos, private.UICamera, nil)
    return localpos
end

return UIManager
