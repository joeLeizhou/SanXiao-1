StateMachine = {}

function StateMachine:New()
    local o = {}
    setmetatable(o, self)
    self.__index = self
    self.states = {}
    self.currentState = nil
    return o
end

function StateMachine:Init(states)
    self:CheckStates(states)
    self.states = states
    self.currentState = nil
end

function StateMachine:CheckStates(states)
    for key, value in pairs(states) do
        if key ~= value.GetStateType() then
            error(key .. " ~= " .. value.GetStateType())
        end
    end
end

function StateMachine:PrintStates()
    for key, value in pairs(self.states) do
        print(key, value.GetStateType())
    end
end

function StateMachine:ChangeState(stateType, ...)
    if not self.states[stateType] then
        error("no state " .. tostring(stateType))
    end
    local lastStateType = self:GetCurrentStateType()
    print("change state " .. tostring(lastStateType) .. "->" .. tostring(stateType))
    if lastStateType == stateType then
        self.currentState.OnRenter(...)
    else
        if self.currentState ~= nil then
            self.currentState.OnExit()
        end
        self.currentState = self.states[stateType]
        self.currentState.OnEnter(...)
    end
end

function StateMachine:GetCurrentStateType()
    return self.currentState and self.currentState.GetStateType() or StateType.NONE
end

function StateMachine:OnUpdate()
    -- print("StateMachine:OnUpdate " .. tostring(self.currentState))
    if self.currentState then
        self.currentState.OnUpdate()
    end
end
