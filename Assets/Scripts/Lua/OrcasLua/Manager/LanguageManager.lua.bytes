CLanguageManager = Orcas.Lua.Core.CLanguageManager;
--C#注册
function CLanguageManager.GetStringByKeyLuaFunc(key, ...)
    local params = {...}
    if #params <= 0 then
        return LanguageManager.GetStringByKey(key)
    else
        return LanguageManager.GetFormatStringByKey(key, ...)
    end
end

LanguageManager = {}
LanguageManager.LanguageKey = "LANGUAGE_SELECT"
LanguageManager.LangugeCfg = PlayerPrefsManager.GetString(LanguageManager.LanguageKey, LocalDataEnum.DT_LanguageEnglish)

function LanguageManager.SetLanguge(lanType)
    PlayerPrefsManager.SetString(LanguageManager.LanguageKey, lanType)
    LanguageManager.LangugeCfg = lanType
    PlayerPrefsManager.Save()
end

LanguageManager.DefaultLanguageSeted = false
function LanguageManager.SetDefaultLanguageIfNeed()
    if LanguageManager.DefaultLanguageSeted == true then
        return
    end
    LanguageManager.DefaultLanguageSeted = true
    if PlayerPrefsManager.GetInt(LanguageManager.LanguageKey .. "_SetNative", 0) ~= 0 then
        return
    end
    -- local nativeLanguage = "Csv" .. SystemManager.getLanguageCode()
    -- nativeLanguage = string.sub(nativeLanguage, 1, 5)
    -- if nativeLanguage == LocalDataEnum.DT_LanguageEnglish or
    --     nativeLanguage == LocalDataEnum.DT_LanguageChinese then
    --     PlayerPrefsManager.SetString(LanguageManager.LanguageKey, nativeLanguage)
    --     LanguageManager.LangugeCfg = nativeLanguage
    -- end
    
    PlayerPrefsManager.SetInt(LanguageManager.LanguageKey .. "_SetNative", 1)

end

LanguageManager.Datas = {}
function LanguageManager.GetLocalDataByType(lan)
    if not LanguageManager.Datas[lan] then
        local json = require("cjson")
        LanguageManager.Datas[lan] = json.decode(ResourcesManager.LoadCurrentLanguage(lan))
    end
    return LanguageManager.Datas[lan]
end

--[Comment]
--获取多语言文本
function LanguageManager.GetStringByKey(key)
    LanguageManager.SetDefaultLanguageIfNeed()
    local langugeData = LanguageManager.GetLocalDataByType(LanguageManager.LangugeCfg)
    if not langugeData[key] then
        --PLog("多语言文本不存在key值" .. key)
        return key
    end
    return langugeData[key]
end

--[Comment]
--获取多语言文本，带参数
function LanguageManager.GetFormatStringByKey(key, ...)
    LanguageManager.SetDefaultLanguageIfNeed()
    local langugeData = LanguageManager.GetLocalDataByType(LanguageManager.LangugeCfg)
    if not langugeData[key] then
        --PLog("多语言文本不存在key值" .. key)
        return key
    end
    local str = string.format(langugeData[key], ...)
    return str
end
