using System.IO;
using UnityEditor;
using UnityEngine;

namespace Orcas.AssetBuilder.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(AssetBundlesBuilderSetting))]
    public class AssetBundlesBuilderSettingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var setting = target as AssetBundlesBuilderSetting;
            if (setting.ExportRoot != null && setting.ExportRoot.Length > 0)
            {
                var lastChar = setting.ExportRoot[setting.ExportRoot.Length - 1];
                if (lastChar != '/' && lastChar != '\\')
                    setting.ExportRoot += "/";
            }

            DrawDefaultInspector();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("资源打包"))
            {
                AssetBundleBuilder.BuildAssetBundles(setting);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("意外中断后还原"))
            {
                AssetBundleBuilder.RevertBuildAssetBundles(setting);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("清空包名"))
            {
                AssetBundleBuilder.ClearAssetBundlesName();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("检查不在Assets的资源"))
            {
                AssetBundleBuilder.CheckDependentNotInAssets(setting);
            }
            EditorGUILayout.EndVertical();
        }
    }
}