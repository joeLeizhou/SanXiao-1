# ToLua Tips

## Delegate

eg:TestDelegate.cs

* 1. add delegate to ExportSettings.customDelegateList
* 2. export delegate
* 3. set delegate in c# :   
    TestEventListener.OnClick onClick =  
    (TestEventListener.OnClick)DelegateTraits<TestEventListener.OnClick>.Create(luafunc);  
* 4. set delegate in lua   
    listener.onClick = TestEventListener.OnClick(t.TestSelffunc, t)  
    listener.onClick = DoClick1


## Int64, UInt64
    in lua 64bit number is userdata type in lua
```
    ------int64------
    local x = int64.new(1, 23)
    local low, high = int64.tonum2(x)
    if int64.equals(x, 123) then
        print('int64 equals to number ok')
    else
        print('int64 equals to number failed')
    end

    local y = int64.new(1)

    x = int64.new('78962871035984074')

    ------uint64------
    x = uint64.new('18446744073709551615')
    low, high = uint64.tonum2(x)  
    
```

## Lua Inherit Extern c# Object

    eg:TestInherit.cs

## Update

    lua/evnet.lua defined UpdateBeat LateUpdateBeat FixedUpdateBeat  
    triggered by lualooper

```
    local handle = UpdateBeat:CreateListener(func, obj)
    UpdateBeat:AddListener(handle)
    UpdateBeat:RemoveListener(handle)
```

## misc/functions.lua

```
    function string.split(input, delimiter)
    function import(moduleName, currentModuleName)
    function reimport(name)
```

## cjson/util.lua

```
    function serialise_value(value, indent, depth)
    function compare_values(val1, val2)
```

