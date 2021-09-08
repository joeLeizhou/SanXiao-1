# ToLua in Orcas

## 一、导出lua层用到的文件

### 1.配置导出目录和导出文件

* "Lua/Gen Custom Export files"，生成配置文件模板。
  保存位置 Editor/CustomExportSetting.cs。

* 设置c#导出文件目录，根据需要修改 outputDir/generateDirbaseTypeDir。
  默认目录为 Application.dataPath + "/Script/CSharp/ToLua/"。
  
* 添加除了预定义之外需要导出的类，根据需要添加。
  默认已添加的类在 Editor/ExportSettings.cs 文件中查看。

### 2.生成BaseType类的wrap文件

* !!!!! 经测试，自动生成的BaseType功能不全，暂不能使用
  
* ~~"Lua/Gen BaseType Wrap"，生成BaseType Wrap。~~

* 有若干个基本类在tolua初始化的时候必须要使用，而随着unity版本的更新，
  这些类可能会发生变化，所以需要重新生成。

* 第一次使用该工具或者unity版本更新，需要重新生成这些wrap文件。

### 3.生成其他类的wrap文件 和delegatefactory文件

* "Lua/Gen Lua Delegates"，只生成Delegate的注册文件，在之后delegate修改时使用。
  
* "Lua/Gen Lua Wrap Files"，只生成类的Wrap文件，在类修改时使用。  
  
* "Lua/Gen LuaWrap + Binder",只生成类的Wrap文件和Binder文件。
   Binder 收集所有wrap文件，方便注册到luastate
  
* "Lua/Generate All"，生成所有 (BaseType 除外)

## 二、lua 使用流程

### 1.自定义LuaClient

* LuaClient 管理 luastate 和 luafilemanager
* "Lua/Gen Lua Client files"， 生成 LuaClient模板

### 2.自定义luafilemanager

* 管理lua文件
* 根据需要可以重写 加载lua脚本流程