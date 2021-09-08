local UIActionMap = {};
function PlayUIActionMotionX(rect, targetPosX, show, callBack)
    if not rect then
        return
    end ;
    if UIActionMap[rect] and UIActionMap[rect] == show then
        return ;
    end
    UIActionMap[rect] = show;
    rect:DOKill();
    if show == true and rect.gameObject.activeSelf == false then
        rect.gameObject:SetActive(true);
    end
    rect:DOAnchorPosX(targetPosX, 0.4):SetEase(DG.Tweening.Ease.OutBack):OnComplete(function()
        if show == false and rect.gameObject.activeSelf == true then
            rect.gameObject:SetActive(false);
        end
        if callBack then
            callBack();
        end
    end);
end

function PlayUIActionMotionY(rect, targetPosY, show, callBack)
    if not rect then
        return
    end ;
    if UIActionMap[rect] and UIActionMap[rect] == show then
        return ;
    end
    UIActionMap[rect] = show;
    rect:DOKill();
    if show == true and rect.gameObject.activeSelf == false then
        rect.gameObject:SetActive(true);
    end
    rect:DOAnchorPosY(targetPosY, 0.4):SetEase(DG.Tweening.Ease.OutBack):OnComplete(function()
        if show == false and rect.gameObject.activeSelf == true then
            rect.gameObject:SetActive(false);
        end
        if callBack then
            callBack();
        end
    end);
end

function PlayUIActionScaleShow(obj, targetSize, show, callBack, onUpdate)
    local ms = obj.transform.localScale;
    obj.transform.localScale = Vector3.New(0, 0, 0);
    if PlayUIActionScale(obj, targetSize, show, callBack, onUpdate) == false then
        obj.transform.localScale = ms;
    end
end

function PlayUIActionScaleHide(obj, show)
    if not obj then
        return
    end ;
    if UIActionMap[obj] and UIActionMap[obj] == show then
        return ;
    end
    UIActionMap[obj] = show;
    if show == false and obj.activeSelf == true then
        obj:SetActive(false);
    end
end

function PlayUIActionScale(obj, targetSize, show, callBack, onUpdate)
    if not obj then
        return false;
    end ;
    if UIActionMap[obj] and UIActionMap[obj] == show then
        return false;
    end
    UIActionMap[obj] = show;
    -- print("UIActionMap[obj] :"..tostring(UIActionMap[obj]));
    obj.transform:DOKill();
    if show == true and obj.activeSelf == false then
        obj:SetActive(true);
    end
    local ease = DG.Tweening.Ease.InBack;
    if show == true then
        ease = DG.Tweening.Ease.OutBack;
    end
    if onUpdate then
        obj .transform:DOScale(targetSize, 0.4):SetEase(ease):OnComplete(function()
            if show == false and obj.activeSelf == true then
                obj:SetActive(false);
            end
            if callBack then
                callBack();
            end
        end):OnUpdate(onUpdate);
    else
        obj.transform:DOScale(targetSize, 0.4):SetEase(ease):OnComplete(function()
            if show == false and obj.activeSelf == true then
                obj:SetActive(false);
            end
            if callBack then
                callBack();
            end
        end);
    end
    return true;
end

function GetUIActionToShow(obj)
    if UIActionMap[obj] and UIActionMap[obj] == true then
        return true;
    end
    return false;
end


function SetAnimationSpeed(animation, name, speed)
    if animation == nil then
        return
    end
    local state = animation.this:get(name)
    if state then
        state.speed = speed
    end
end