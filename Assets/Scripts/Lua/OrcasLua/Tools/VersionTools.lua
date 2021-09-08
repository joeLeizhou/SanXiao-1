--[Comment]
-- Android和iOS版本比较。-1为当前应用版本小于参数，0为等于，1为大于
function CompareApplicationVersionForAndroidAndiOS(va, vi)
    if SystemInfoHelper.GetPlatform() == SystemInfoHelper.Android then
        return CompareApplicationVersion(va)
    elseif SystemInfoHelper.GetPlatform() == SystemInfoHelper.iOS then
        return CompareApplicationVersion(vi)
    else
        return CompareApplicationVersion(va)
    end
    return 0
end

--[Comment]
-- 版本比较。-1为当前应用版本小于参数，0为等于，1为大于
function CompareApplicationVersion(v)
    local version = UnityEngine.Application.version
    return CompareVersionCode(v, version)
end

--[Comment]
-- 当前iOS系统版本比较。-1为当前系统版本小于参数，0为等于，1为大于
function CompareIPhoneOsVersion(strVersion)
    if SystemInfoHelper.GetPlatform() == SystemInfoHelper.iOS then
        -- 待比较
        local toCompareArr = string.split(strVersion, ".");
        if toCompareArr == nil or #strVersion ~= 2 then return -1 end
        
        local strOS = SystemInfoHelper.GetOperatingSystem()
        local tempOSArr = string.split(strOS, " ")
        if tempOSArr ~= nil and #tempOSArr > 0 then
            local strVersionCode = tempOSArr[#tempOSArr]
            local tempVersionArr = string.split(strVersionCode, ".")
            if tempVersionArr ~= nil and #tempVersionArr == 2 then
                local toCompare1 = tonumber(toCompareArr[1]);
                local toCompare2 = tonumber(toCompareArr[2]);
                local compare1 = tonumber(tempVersionArr[1]);
                local compare2 = tonumber(tempVersionArr[2]);
                if compare1 == toCompare1 and compare2 == toCompare2 then
                    return 0;
                elseif compare1 > toCompare1 or (compare1 == toCompare1 and compare2 > toCompare2) then
                    return 1;
                end
            end
        end
    end
    return -1;
end


CompareVersionCode = function(va, vb)
    local versionAList = {}
    local versionBList = {}
    local regxEverythingExceptComma = '([^.]+)'
    for x in string.gmatch(va, regxEverythingExceptComma) do
        versionAList[#versionAList + 1] = tonumber(x)
    end
    for x in string.gmatch(vb, regxEverythingExceptComma) do
        versionBList[#versionBList + 1] = tonumber(x)
    end
    local maxLen = math.min(#versionAList, #versionBList)
    for i = 1, maxLen do
        if versionBList[i] > versionAList[i] then
            return -1
        elseif versionBList[i] < versionAList[i] then
            return 1
        end
    end
    if #versionBList > maxLen then
        return -1
    elseif #versionAList > maxLen then
        return 1
    else
        return 0
    end
end

function CheckVersionCodeNeedHotFix(hotfixVer, forceFixVer)
    local versionCode = SystemInfoHelper.GetVersionCode()
    return CompareVersionCode(hotfixVer, versionCode) > 0 or CompareVersionCode(forceFixVer, versionCode) > 0
end