using System;
using UnityEditor;
using UnityEngine;

namespace Orcas.Resources.Editor
{
    [CustomEditor(typeof(ProjectBuilderSetting))]
    class ProjectBuilderSettingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var setting = target as ProjectBuilderSetting;
            DrawDefaultInspector();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("准备出包"))
            {
                ProjectBuilder.PrepearBuild(setting);
            }

            GUILayout.Space(30);
            if (GUILayout.Button("拷贝streamingasset出包目录(出包之后只更新ab使用)"))
            {
                ProjectBuilder.CopyStreamingAssetToBuild(setting);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
