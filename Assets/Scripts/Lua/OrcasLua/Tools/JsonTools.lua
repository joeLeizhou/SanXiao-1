local json = require("cjson")

function JsonSafeEncode(str)
    local data = nil
    -- status:true/false 
    -- result:data/error message 
    local status, result = pcall(function(str1)
        return json.encode(str1)
    end, str)
    if status == true then
        data = result
    end
    return data
end

function JsonSafeDecode(dic)
    local str = ""
    local status, result = pcall(function(dic1)
        return json.decode(dic1)
    end, dic)
    
    if status == true then
        str = result
    end
    return str
end

function IsNullJson(s)
    return s == nil or s == json.null
end

function PrintJsonLog(tag, obj)
    print(tag .. "\n" .. JsonSafeEncode(obj))
end