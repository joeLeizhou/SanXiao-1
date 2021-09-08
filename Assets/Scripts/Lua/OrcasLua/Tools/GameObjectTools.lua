--[Comment]
-- 删除所有子节点
function DeleteAllChildren(parent)
    for i = 1, parent.transform.childCount do
        local child = parent.transform.GetChild(i)
        GameObject.Destroy(child.gameObject)
    end
end

--[Comment]
-- 显示和隐藏物体
function SetObjectActive(obj, active)
    if obj and obj.activeSelf ~= active then
        obj:SetActive(active);
    end
end

--[Comment]
-- 删除节点下，名字包含SubName的所有物体
function DeleteAllNameContains(ParentTrans, SubName)
    if not ParentTrans then
        return
    end   --不指定父节点则不执行删除
    local ChildCount = ParentTrans.childCount
    for i = 1, ChildCount do
        if string.find(ParentTrans:GetChild(i - 1).name, SubName, 1) then
            UnityEngine.GameObject.Destroy(ParentTrans:GetChild(i - 1).gameObject)
        end
    end
end


function CreateGameObject(path, parent, pos, scale)
    local go = Orcas.Resources.ResourceLoader.LoadGameObject(path)
    go.transform:SetParent(parent, false)
    go.transform.localPosition = pos or Vector3.zero
    go.transform.localScale = scale or Vector3.one
    return go;
end

function SetChildAtIndexActive(Parent, Index)
    for i = 1, Parent.transform.childCount do
        local g = Parent.transform:GetChild(i - 1).gameObject
        SetObjectActive(g, i == Index)
    end
end


function SetParticleScale(root, scale)
    local particleSystem = root.gameObject:GetComponent("ParticleSystem")
    if particleSystem then
        particleSystem.gameObject.transform.localScale = scale
    end
    local childCount = root.gameObject.transform.childCount
    for i = 0, childCount - 1 do
        local child = root.gameObject.transform:GetChild(i)
        SetParticleScale(child, scale)
    end
end

function SetParticleDepth(root, depth, higherNames)
    higherNames = higherNames or {}
    local addLayer = 0
    local particleSystem = root.gameObject:GetComponent("ParticleSystem")
    if particleSystem then
        local renderer = particleSystem:GetComponent("ParticleSystemRenderer")
        if renderer ~= nil then
            renderer.sortingOrder = depth
            for i = 1, #higherNames do
                for j = 1, #higherNames[i] do
                    if renderer.gameObject.name == higherNames[i][j] then
                        renderer.sortingOrder = depth + i
                        addLayer = i
                        break
                    end
                end
            end
        end
    end
    local childCount = root.gameObject.transform.childCount
    for i = 0, childCount - 1 do
        local child = root.gameObject.transform:GetChild(i)
        SetParticleDepth(child, depth + addLayer, higherNames)
    end
end


function IsNil(uobj)
    return uobj == nil or uobj:Equals(nil)
end