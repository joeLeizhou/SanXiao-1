MessageEventManager = {}

--Comment
--存储注册事件
MessageEventManager.funcs = {}

--Comment
--添加服务器消息监听，也可以当做本监听使用，暂时只用作服务器消息使用(参数类型：dataType, instanceId, func)
MessageEventManager.AddMessageListener = function (dataType, instanceId, func)
    if MessageEventManager.funcs[dataType] == nil then
        MessageEventManager.funcs[dataType] = {}        
    end
    if MessageEventManager.funcs[dataType][instanceId] ~= nil then
        print("[MessageEventManager] dataType "..dataType.." instanceId "..instanceId.." override !! ", "#FF0000")
    end
    MessageEventManager.funcs[dataType][instanceId] = func
    -- PrintLog("注册消息:" .. dataType .. "  " .. instanceId);
end

--Comment
--删除服务器消息监听，也可以当做本监听使用，暂时只用作服务器消息使用(参数类型：dataType, instanceId)
MessageEventManager.RemoveMessageListener = function (dataType, instanceId)
    if MessageEventManager.funcs[dataType] ~= nil then
		MessageEventManager.funcs[dataType][instanceId] = nil
	end
    if MessageEventManager.funcs[dataType] and _G.next(MessageEventManager.funcs[dataType]) == nil then
        MessageEventManager.funcs[dataType] = nil
    end
end

--Comment
--删除某一类型的全部消息监听(参数类型：dataType)
MessageEventManager.RemoveTypeMessageListener = function(dataType)
	MessageEventManager.funcs[dataType] = nil
end

--Comment
--广播服务器消息(参数类型：dataType, ...)
MessageEventManager.BroadcastMessage = function (dataType, ...)
    if MessageEventManager.funcs[dataType] then
		for key, func in pairs(MessageEventManager.funcs[dataType]) do
			func(...) 
		end
    end
end


