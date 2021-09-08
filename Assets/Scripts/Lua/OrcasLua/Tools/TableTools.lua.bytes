--[Comment]
--按整数索引遍历寻找是否包含对应的元素
function table.icontains(tb, val)
    if val == nil then
        return false
    end
    for i = 1, #tb do
        if tb[i] == val then
            return true
        end
    end
    return false
end

--[Comment]
--遍历寻找是否包含对应的元素
function table.contains(tb, val)
    if val == nil then
        return false
    end
    for k, v in pairs(tb) do
        if v == val then
            return true
        end
    end
    return false
end

--[Comment]
--遍历寻找对应的元素的key
function table.find(tb, val)
    if val == nil then
        return nil
    end
    for k, v in pairs(tb) do
        if v == val then
            return k
        end
    end
    return nil
end

--[Comment]
--遍历计算table的元素个数
function table.count(dictTable)
    local targetCount = 0
    for k, v in pairs(dictTable) do
        targetCount = targetCount + 1
    end
    return targetCount
end

--[Comment]
--字典转数组
function table.toArray(tb)
    local result = {};
    if tb then
        for k, v in pairs(tb) do
            table.insert(result, v);
        end
    end
    return result
end

--[Comment]
--对本地数据进行排序
function SortDataTableByKey (dataTable, keyName, upward)
    local key = keyName ~= nil and keyName or "id"
    local sortTable = {};
    for id, data in pairs(dataTable) do
        sortTable[#sortTable + 1] = data;
    end

    if upward then
        table.sort(sortTable, function(tabA, tabB)
            return tabA[key] > tabB[key]
        end);
    else
        table.sort(sortTable, function(tabA, tabB)
            return tabA[key] < tabB[key]
        end);
    end

    return sortTable;
end

--[Comment]
--顺序遍历table时用的迭代器
function PairsByKeys(t)
    local a = {}
    for n in pairs(t) do
        a[#a + 1] = n
    end
    table.sort(a)
    local i = 0
    return function()
        i = i + 1
        return a[i], t[a[i]]
    end
end

-- 深拷贝对象
function table.clone(object)
    local lookup_table = {}
    
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        
        local new_table = {}
        lookup_table[object] = new_table
        for index, value in pairs(object) do
            new_table[_copy(index)] = _copy(value)
        end
        
        return setmetatable(new_table, getmetatable(object))
    end
    
    return _copy(object)
end