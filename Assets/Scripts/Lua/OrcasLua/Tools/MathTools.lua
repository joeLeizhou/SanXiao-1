--[Comment]
--截取小数后几位
function GetPreciseDecimal(nNum, n)
    if type(nNum) ~= "number" then
        return nNum;
    end
    n = n or 0;
    n = math.floor(n)
    if n < 0 then
        n = 0;
    end
    local nDecimal = 10 ^ n
    local nTemp = math.floor(nNum * nDecimal);
    local nRet = nTemp / nDecimal;
    return nRet;
end

--[Comment]
--a绕b旋转angle度,弧度,b最好是单位向量
function RotateVector3ByVector3(a, b, angle)
    local cosAngle = Mathf.Cos(angle);
    return a * cosAngle + Vector3.Cross(b, a) * Mathf.Sin(angle) + b * Vector3.Dot(b, a) * (1 - cosAngle);
end

--[Comment]
--二阶贝塞尔曲线
function Bezier(p0, p1, p2, t)
    return p0 * (1 - t) * (1 - t) + p1 * 2 * t * (1 - t) + p2 * t * t;
end

--[Comment]
--二阶贝塞尔正切值
function BezierTangent(p0, p1, p2, t)
    return p0 * (t - 1) * 2 + p1 * 2 * (1 - 2 * t) + p2 * 2 * t;
end

--[Comment]
--三阶贝塞尔曲线
function Bezier3(p0, p1, p2, p3, t)
    return p0 * (1 - t) * (1 - t) * (1 - t) + p1 * t * (1 - t) * (1 - t) * 3 + p2 * t * t * (1 - t) * 3 + p3 * t * t * t;
end

--[Comment]
--检查数值在范围内
function CheckValueBetween(value, min, max)
    if value < min then
        return min
    elseif value > max then
        return max
    end
    return value
end

--[Comment]
--转字符串取整，专门解决Lua浮点精度截断问题，其他情况慎用
function PreciseFloatToInt(s)
    local i
    local j
    local temp
    i = tostring(s)
    j = string.find(i, "%.", 1)
    if j ~= nil and j ~= 1 then
        temp = string.sub(s, 1, j - 1)
    else
        temp = i
    end
    return tonumber(temp);
end

function NormalNum(num)
    if num == nil then
        return "0"
    end
    if num >= 1000000 then
        return tostring(num / 1000000) .. "M"
    elseif num >= 1000 then
        return tostring(num / 1000) .. "K"
    end
    return tostring(num)
end


function Bool2Int (b)
    if b then
        return 1
    else
        return 0
    end
end