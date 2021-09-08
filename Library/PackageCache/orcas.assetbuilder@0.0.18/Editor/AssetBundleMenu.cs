using System.IO;
using UnityEngine;
using UnityEditor;

namespace Orcas.AssetBuilder.Editor
{
    public class AssetBundleMenu
    {
        [MenuItem("Assets/Create/工具/资源打包配置")]
        public static void CreateConfigure()
        {
            var configure = ScriptableObject.CreateInstance<AssetBundlesBuilderSetting>();
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            Selection.activeObject = configure;
            var baseName = "New AssetBundles Build Settings";

            configure.name = baseName;
            var aPath = path + "/" + configure.name + ".asset";
            aPath = AssetDatabase.GenerateUniqueAssetPath(aPath);
            AssetDatabase.CreateAsset(configure, aPath);
            AssetDatabase.SaveAssets();
        }
    }
}

