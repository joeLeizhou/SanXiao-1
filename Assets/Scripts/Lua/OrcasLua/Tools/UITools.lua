--[Comment]
-- 对齐方式
AnchorPresets = {
    TopLeft = 1,
    TopCenter= 2,
    TopRight= 3,

    MiddleLeft= 4,
    MiddleCenter= 5,
    MiddleRight= 6,

    BottomLeft= 7,
    BottomCenter= 8,
    BottomRight= 9,
    BottomStretch= 10,

    VertStretchLeft= 11,
    VertStretchRight= 12,
    VertStretchCenter= 13,

    HorStretchTop= 14,
    HorStretchMiddle= 15,
    HorStretchBottom= 16,

    StretchAll= 17
}

--[Comment]
-- 设置对齐方式
function SetAnchor(source, align, offsetX, offsetY)
    source.anchoredPosition = Vector3.New(offsetX, offsetY, 0);
    if (AnchorPresets.TopLeft == align) then
        source.anchorMin = Vector2.New(0, 1);
        source.anchorMax = Vector2.New(0, 1);
    elseif (AnchorPresets.TopCenter == align) then
        source.anchorMin = Vector2.New(0.5, 1);
        source.anchorMax = Vector2.New(0.5, 1);
    elseif (AnchorPresets.TopRight == align) then
        source.anchorMin = Vector2.New(1, 1);
        source.anchorMax = Vector2.New(1, 1);
    elseif (AnchorPresets.MiddleLeft == align) then
        source.anchorMin = Vector2.New(0, 0.5);
        source.anchorMax = Vector2.New(0, 0.5);
    elseif (AnchorPresets.MiddleCenter == align) then
        source.anchorMin = Vector2.New(0.5, 0.5);
        source.anchorMax = Vector2.New(0.5, 0.5);
    elseif (AnchorPresets.MiddleRight == align) then
        source.anchorMin = Vector2.New(1, 0.5);
        source.anchorMax = Vector2.New(1, 0.5);
    elseif (AnchorPresets.BottomLeft == align) then
        source.anchorMin = Vector2.New(0, 0);
        source.anchorMax = Vector2.New(0, 0);
    elseif (AnchorPresets.BottomCenter == align) then
        source.anchorMin = Vector2.New(0.5, 0);
        source.anchorMax = Vector2.New(0.5, 0);
    elseif (AnchorPresets.BottomRight == align) then
        source.anchorMin = Vector2.New(1, 0);
        source.anchorMax = Vector2.New(1, 0);
    elseif (AnchorPresets.HorStretchTop == align) then
        source.anchorMin = Vector2.New(0, 1);
        source.anchorMax = Vector2.New(1, 1);
    elseif (AnchorPresets.HorStretchMiddle == align) then
        source.anchorMin = Vector2.New(0, 0.5);
        source.anchorMax = Vector2.New(1, 0.5);
    elseif (AnchorPresets.HorStretchBottom == align) then
        source.anchorMin = Vector2.New(0, 0);
        source.anchorMax = Vector2.New(1, 0);
    elseif (AnchorPresets.VertStretchLeft == align) then
        source.anchorMin = Vector2.New(0, 0);
        source.anchorMax = Vector2.New(0, 1);
    elseif (AnchorPresets.VertStretchCenter == align) then
        source.anchorMin = Vector2.New(0.5, 0);
        source.anchorMax = Vector2.New(0.5, 1);
    elseif (AnchorPresets.VertStretchRight == align) then
        source.anchorMin = Vector2.New(1, 0);
        source.anchorMax = Vector2.New(1, 1);
    elseif (AnchorPresets.StretchAll == align) then
        source.anchorMin = Vector2.New(0, 0);
        source.anchorMax = Vector2.New(1, 1);
    end
end

--[Comment]
-- 显示和隐藏物体
function SetSortingOrder(obj, depth)
    local tD = obj:GetComponent("UIDepthControl")
    if tD then
        tD.enabled = true
        tD.depth = depth
        tD.isUI = true

        local tC = obj:GetComponent("Canvas")
        if tC then
            tC.enabled = true
            tC.sortingOrder = depth
        end
    else
        tD = obj:AddComponent(typeof(UIDepthControl))
        tD.depth = depth
        tD.isUI = true
        tD.enabled = true
    end
end
