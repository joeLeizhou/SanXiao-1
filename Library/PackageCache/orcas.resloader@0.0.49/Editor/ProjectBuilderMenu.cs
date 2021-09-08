using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Orcas.Resources.Editor
{
    public class ProjectBuilderMenu
    {
        [MenuItem("Assets/Create/工具/出包配置")]
        public static void CreateProjectConfigure()
        {
            var configure = ScriptableObject.CreateInstance<ProjectBuilderSetting>();
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            Selection.activeObject = configure;
            var baseName = "New Project Build Settings";

            configure.name = baseName;
            var aPath = path + "/" + configure.name + ".asset";
            aPath = AssetDatabase.GenerateUniqueAssetPath(aPath);
            AssetDatabase.CreateAsset(configure, aPath);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/工具/热更配置")]
        public static void CreateHotfixConfigure()
        {
            var configure = ScriptableObject.CreateInstance<HotfixBuilderSetting>();
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            Selection.activeObject = configure;
            var baseName = "New Hotfix Build Settings";

            configure.name = baseName;
            var aPath = path + "/" + configure.name + ".asset";
            aPath = AssetDatabase.GenerateUniqueAssetPath(aPath);
            AssetDatabase.CreateAsset(configure, aPath);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/工具/分包配置")]
        public static void CreatePackConfigure()
        {
            var configure = ScriptableObject.CreateInstance<PackExportSetting>();
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            Selection.activeObject = configure;
            var baseName = "New Pack Export Settings";

            configure.name = baseName;
            var aPath = path + "/" + configure.name + ".asset";
            aPath = AssetDatabase.GenerateUniqueAssetPath(aPath);
            AssetDatabase.CreateAsset(configure, aPath);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("/Orcas/DebugTool/Clear Saved Version")]
        public static void ClearVersion()
        {
            PlayerPrefs.DeleteKey("hot_version");
            PlayerPrefs.DeleteKey("build_guid");
            PlayerPrefs.Save();
            File.Delete(PathConst.DestPath + PathConst.FileListName);
        }

        [MenuItem("/Orcas/DebugTool/Clear PersistentDataPath")]
        public static void ClearPersistentDataPath()
        {
            Debug.Log("delete " + PathConst.DestPath);
            Directory.Delete(PathConst.DestPath, true);
        }
    }
}