using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Orcas.AssetBuilder;
using Orcas.AssetBuilder.Editor;
using System.Collections.Generic;

namespace Orcas.Resources.Editor
{
    [CustomEditor(typeof(PackExportSetting))]
    class PackExportSettingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var setting = target as PackExportSetting;
            setting.ExportRoot = PathConst.CheckAndFixPathRoot(setting.ExportRoot);

            DrawDefaultInspector();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("导出分包资源"))
            {
                PackExporter.ExportPacks(setting);
            }
            if (GUILayout.Button("清理不使用AB文件"))
            {
                PackExporter.DeleveUnuseFile(setting);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
