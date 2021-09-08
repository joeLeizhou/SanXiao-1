TimeManager = {}
local private = {}

TimeManager.SetServerTime = function(time)
    private.serverTime = time
    private.lastSetTime = os.time()
end

TimeManager.CurrentServerTime = function()
    if private.serverTime then
        return private.serverTime + (os.time() - private.lastSetTime)
    end
    return 0
end

--[Comment]
--得到当前table类型时间
TimeManager.GetCurrentDateTime = function ()
    local curDateTime = TimeManager.GetDateTime(TimeManager.CurrentServerTime())
    --curDateTime:为table类型包含的字段（sec, day, wday, hour, min, month, year, yday, isdst = false）
    return curDateTime
end

--[Comment]
--返回星期数目，--wDay取值: 1:星期日 2：星期一 3：星期二 ...以此类推
TimeManager.TransformWeekDay = function (wDay)
    return wDay == 1 and 7 or wDay - 1
end

--[Comment]
--将秒数转换为table类型时间
TimeManager.GetDateTime = function (timeStamp)
    local curTime = timeStamp
    local curDateTime = os.date("*t", curTime)
    --curDateTime:为table类型包含的字段（sec, day, wday, hour, min, month, year, yday, isdst = false）
    --wDay取值: 1:星期日 2：星期一 3：星期二 ...以此类推
    return curDateTime
end

--[Comment]
--得到当前时间距离将来某一时间的差值(倒计时一般不会按月份最大单位倒计时的，顶多天数)
TimeManager.GetDeltaTime = function (willTime) 
    local deltaTime = willTime - TimeManager.CurrentServerTime()
    return TimeManager.GetDeltaTableTime(deltaTime)
end
--[Comment]
--据根deltaTime毫秒数转换为table类型时间(倒计时一般不会按月份最大单位倒计时的，顶多天数)
TimeManager.GetDeltaTableTime = function (deltaTime)
    deltaTime = deltaTime <= 0 and 0 or deltaTime
    local resultTb = {}
    resultTb.totalSec = math.ceil(deltaTime)
    resultTb.day =  math.floor(resultTb.totalSec/86400)
    resultTb.hour = math.floor((resultTb.totalSec%86400)/3600)
    resultTb.min = math.floor((resultTb.totalSec%3600)/60)
    resultTb.sec = math.floor(resultTb.totalSec%60)
    return resultTb    
end

return TimeManager