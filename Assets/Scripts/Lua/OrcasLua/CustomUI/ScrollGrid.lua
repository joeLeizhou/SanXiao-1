local setmetatable = setmetatable
local LeftTopAnchors = Vector2.New(0, 1)
local MiddleTopAnchors = Vector2.New(0.5, 1)

ScrollGrid = {}

local Padding = {
    Left = 0,
    Right = 0,
    Top = 0,
    Buttom = 0,
}

local Spacing = {
    x = 0,
    y = 0,
}

function ScrollGrid:Init(scrollRect, component, dataList, size, spacing, padding, horizontalNumber, sizeFunction)
    if (spacing == nil) then
        spacing = Spacing
    end
    if (padding == nil) then
        padding = Padding
    end
    if (horizontalNumber == nil) then
        horizontalNumber = 1
    end

    self.ScrollRect = scrollRect;
    self.Viewport = scrollRect.viewport;
    self.Content = scrollRect.content;
    self.Component = component
    self.DataList = dataList
    self.Spacing = spacing
    self.Padding = padding
    self.HorizontalNumber = horizontalNumber
    self.ItemSize = size
    self.SizeFunction = sizeFunction
    self.componentList = {}
    self.componentPool = {}
    self.CellInfos = {}
    self.pos = Vector2.New(0, 0)
    UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(self.Viewport)
    self:AddScrollEvent()
    self:UpdateList(dataList)
    self.isInited = true
end

function ScrollGrid:AddScrollEvent()
    UIEventManager.Get(self.ScrollRect.gameObject):ScrollOnValueChanged(function(value)
        local isUp = self.pos.y > value.y;
        self.pos = value;

        if (self.CellInfos == nil) then
            return ;
        end
        local startState = 0;
        if (isUp) then
            for i = self.minIndex, #self.CellInfos do
                if (self:CheckOutRangeToCell(i)) then
                    if (startState == 1) then
                        break ;
                    end
                else
                    if (startState == 0) then
                        self.minIndex = i;
                        startState = 1;
                    end
                    self.maxIndex = i;
                end
            end
        else
            for i = self.maxIndex, 1, -1 do
                if (self:CheckOutRangeToCell(i)) then
                    if (startState == 1) then
                        break ;
                    end
                else
                    if (startState == 0) then
                        self.maxIndex = i;
                        startState = 1;
                    end
                    self.minIndex = i;
                end
            end
        end
    end)
end

function ScrollGrid:RemoveScrollEvent()
    UIEventManager.Get(self.ScrollRect.gameObject):RemoveScrollOnValueChanged()
end

function ScrollGrid:UpdateList(dataList, IsUpdateData)
    self.InsertData = {}
    if (self.isInited) then
        for i = #self.CellInfos, 1, -1 do
            self:SetPoolsObj(self.CellInfos[i].obj);
            self.CellInfos[i].obj = nil;
        end
    end

    local count = 1
    for i, v in pairs(dataList) do
        if (self.CellInfos[count] == nil) then
            self.CellInfos[count] = {}
        end
        self.CellInfos[count].data = v
        if(self.CellInfos[count].size == nil or not IsUpdateData) then
            if (self.SizeFunction ~= nil) then
                self.CellInfos[count].size = self.SizeFunction(v)
            else
                self.CellInfos[count].size = { x = self.ItemSize.x, y = self.ItemSize.y }
            end
        end
        count = count + 1
    end

    for i = #self.CellInfos, count, -1 do
        table.remove(self.CellInfos, i)
    end
    self:Relayout(IsUpdateData)
end

function ScrollGrid:Relayout(isUpdateData)
    local maxY, temporaryY = 0
    local currentSize = { x = self.Padding.Left, y = self.Padding.Top }
    for i = 1, #self.CellInfos do

        if self.InsertData and _G.next(self.InsertData) ~= nil then
            for k,v in pairs(self.InsertData) do
                if v.index == i then
                    v.obj.transform.anchoredPosition = Vector2.New(currentSize.x, -currentSize.y)
                    if v.vertical then
                        currentSize.y = currentSize.y + v.height
                    elseif v.horizonal then
                        currentSize.x = currentSize.x + v.width
                    end
                end
            end
        end

        self.CellInfos[i].pos = { x = currentSize.x, y = -currentSize.y }

        currentSize.x = currentSize.x + self.CellInfos[i].size.x + self.Spacing.x;
        temporaryY = currentSize.y + self.CellInfos[i].size.y;
        maxY = temporaryY > maxY and temporaryY or maxY;

        if (i % self.HorizontalNumber == 0) then
            currentSize.x = self.Padding.Left;
            currentSize.y = maxY + self.Spacing.y;
        end

        self:CheckOutRangeToCell(i, isUpdateData)
    end

    self.maxIndex = #self.CellInfos
    self.minIndex = 1

    currentSize.x = currentSize.x + self.Padding.Right
    currentSize.y = currentSize.y + self.Padding.Buttom
    if #self.CellInfos % self.HorizontalNumber ~= 0 then
        currentSize.y = currentSize.y + self.ItemSize.y
    else
        currentSize.y = currentSize.y - self.Spacing.y
    end
    local contentWidth = self.Content.sizeDelta.x;
    self.Content.sizeDelta = Vector2.New(contentWidth, currentSize.y)
    UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(self.Content)

    -- 如果定位不在屏幕范围内，设置anchoredPosition，上对齐或下对齐
    if isUpdateData and self.Content.rect.height - self.Content.anchoredPosition.y < self.Viewport.rect.height then
        local anchoredPosition = self.Content.anchoredPosition
        anchoredPosition.y = Mathf.Max(self.Content.rect.height - self.Viewport.rect.height, 0)
        self.Content.anchoredPosition = anchoredPosition
    end
end

function ScrollGrid:CheckOutRangeToCell(i, isUpdateData)
    local cellInfo = self.CellInfos[i]
    local obj = cellInfo.obj
    local pos = cellInfo.pos
    --判断是否超出显示范围
    if (self:IsOutRange(pos.y, cellInfo.size.y)) then
        if (obj ~= nil) then
            self:SetPoolsObj(obj);
            self.CellInfos[i].obj = nil;
        end
        return true;
    else
        if (obj == nil) then
            --优先从 poolsObj中 取出 （poolsObj为空则返回 实例化的cell）
            local cell = self:GetPoolsObj();
            local rect = cell.gameObject.transform:GetComponent("RectTransform")
            rect.pivot = LeftTopAnchors;
            rect.anchorMin = MiddleTopAnchors;
            rect.anchorMax = MiddleTopAnchors;
            rect.localPosition = Vector3.New(pos.x, pos.y, 0);
            self.CellInfos[i].obj = cell;
            cell.SetData(self.CellInfos[i], i)
        else
            local rect = cellInfo.obj.gameObject.transform:GetComponent("RectTransform")
            rect.localPosition = Vector3.New(pos.x, pos.y, 0);
            rect.sizeDelta = Vector2.New(cellInfo.size.x, cellInfo.size.y)
            if (isUpdateData) then
                obj.SetData(self.CellInfos[i], i)
            end
        end
        return false;
    end
end

function ScrollGrid:IsOutRange(pos, hight)
    local listP = self.Content.anchoredPosition;
    if (pos + listP.y > hight or pos + listP.y < -self.Viewport.rect.height) then
        return true;
    end
    return false
end

function ScrollGrid:SetPoolsObj(cell)
    if (cell ~= nil) then
        table.insert(self.componentPool, cell)
        cell.OnHide();
    end
end

function ScrollGrid:GetPoolsObj()
    local poolLength = #self.componentPool
    local component = nil
    if (poolLength > 0) then
        component = self.componentPool[poolLength]
        component.OnShow()
        table.remove(self.componentPool, poolLength)
    end

    if component == nil then
        component = UIManager.CreateComponent(self.Component, self.Content.gameObject)
        table.insert(self.componentList, component)
    end
    return component
end

function ScrollGrid:GetTotalHeight()
    return self.Content.sizeDelta.y
end

function ScrollGrid:ScrollToIndex(index)
    local pos = self.Content.anchoredPosition
    pos.y = -self.CellInfos[index].pos.y
    self.Content.anchoredPosition = pos
    self:Relayout(true)
end

function ScrollGrid:ScrollToY(posY)
    local pos = self.Content.anchoredPosition
    pos.y = posY
    self.Content.anchoredPosition = pos
    self:Relayout(true)
end

function ScrollGrid:Insert(index, width, height, obj, horizonal, vertical)
    local result = {}
    result.index = index;
    result.width = width and width or 0;
    result.height = height and height or 0;
    result.horizonal = horizonal;
    result.vertical = vertical;
    result.obj = obj
    table.insert(self.InsertData, result)
    self:Relayout()
end

function ScrollGrid:Update(deltaTime)
    for i = 1, #self.componentList do
        if (self.componentList[i].IsEnabled) then
            self.componentList[i].Update(deltaTime)
        end
    end
end

function ScrollGrid:UpdateBySecond(time)
    for i = 1, #self.componentList do
        if (self.componentList[i].IsEnabled) then
            self.componentList[i].UpdateBySecond(time)
        end
    end
end

function ScrollGrid:Close()
    self:RemoveScrollEvent()
    for i = 1, #self.componentList do
        self.componentList[i].OnDestroy()
    end
    self.componentList = {}
end

ScrollGrid.__call = function(scrollRect, component, dataList, size, spacing, padding, horizontalNumber, sizeFunction)
    local object = setmetatable({}, { __index = ScrollGrid })
    object:Init(scrollRect, component, dataList, size, spacing, padding, horizontalNumber, sizeFunction)
    return object
end

ScrollGrid.New = function(scrollRect, component, dataList, size, spacing, padding, horizontalNumber, sizeFunction)
    local object = setmetatable({}, { __index = ScrollGrid })
    object:Init(scrollRect, component, dataList, size, spacing, padding, horizontalNumber, sizeFunction)
    return object
end

return ScrollGrid