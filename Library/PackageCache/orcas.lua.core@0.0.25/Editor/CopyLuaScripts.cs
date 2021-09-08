using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Orcas.Lua.Core.Editor
{
    public class CopyLuaScripts
    {
         [MenuItem("Orcas/Lua/拷贝Lua脚本到工程目录", false, 1)]
         public static void CopyLuaScriptsToProject()
         {
             
             if (EditorUtility.DisplayDialog("警告", "拷贝操作会直接覆盖整个目录，确定要覆盖吗？", "确定", "取消"))
             {
                 string dstFullPath = Path.GetFullPath("Assets/Scripts/Lua/OrcasLua");
                 bool dirExist = Directory.Exists(dstFullPath);
             
                 if (dirExist)
                 {
                     Directory.Delete(dstFullPath, true);
                 }
                 string srcPath = Path.GetFullPath("Packages/orcas.lua.core/Editor/LuaScripts/");
                 DfsPath(srcPath, (dir, filePath) =>
                 {
                     if (Path.GetExtension(filePath).ToLower() != ".meta" && Path.GetExtension(filePath).ToLower() != ".ds_store"){
                         int relativePathIndex = filePath.LastIndexOf("LuaScripts");
                         string relativePath = filePath.Substring(relativePathIndex + 11);
                         string dstFile = Path.GetFullPath("Assets/Scripts/Lua/OrcasLua/" + relativePath);


                         relativePathIndex = dir.LastIndexOf("LuaScripts");
                         relativePath = dir.Substring(relativePathIndex + 11);
                         if (!string.IsNullOrEmpty(relativePath))
                         {
                             string dstDir = Path.GetFullPath("Assets/Scripts/Lua/OrcasLua/" + relativePath);
                             if (!Directory.Exists(dstDir))
                             {
                                 Directory.CreateDirectory(dstDir);
                             }
                         }
                         File.Copy(filePath, dstFile, true);
                     }
                 });
                 AssetDatabase.Refresh();
                 Debug.Log("Orcas Lua Core 拷贝完成");
             }
         }
         
         public static void DfsPath(string dir, Action<string, string> callBack){
             foreach (string path in Directory.GetFiles(dir))  {
                 callBack(dir, path);
             }
             foreach (string path in Directory.GetDirectories(dir))  {
                 DfsPath(path, callBack);
             }
         }
    }
}
