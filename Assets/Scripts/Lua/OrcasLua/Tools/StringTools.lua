
--[Comment]
--字符串转char数组
function string.toCharArray(str)
    str = str or ""
    local array = {}
    local len = string.len(str)
    while str do
        local fontUTF = string.byte(str, 1)

        if fontUTF == nil then
            break
        end

        local byteCount = 0
        if fontUTF > 0 and fontUTF <= 127 then
            byteCount = 1
        elseif fontUTF >= 192 and fontUTF < 223 then
            byteCount = 2
        elseif fontUTF >= 224 and fontUTF < 239 then
            byteCount = 3
        elseif fontUTF >= 240 and fontUTF <= 247 then
            byteCount = 4
        end
        local tmp = string.sub(str, 1, byteCount)
        table.insert(array, tmp)
        str = string.sub(str, byteCount + 1, len)
    end
    return array
end

--[Comment]
--子串最后出现位置
function string.lastIndexOf(haystack, needle)
    local i = haystack:match(".*" .. needle .. "()")
    if i == nil then
        return nil
    else
        return i - 1
    end
end

--[Comment]
--字符串是否为nil或空字符串
function string.isNullOrEmpty(str)
    local result = str == nil or string.len(str) <= 0
    return result
end