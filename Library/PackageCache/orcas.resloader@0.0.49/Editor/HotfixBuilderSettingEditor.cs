using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Orcas.Resources.Editor
{
    [CustomEditor(typeof(HotfixBuilderSetting))]
    class HotfixBuilderSettingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var setting = target as HotfixBuilderSetting;
            DrawDefaultInspector();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("生成热更"))
            {
                HotfixBuilder.BuildHotfix(setting);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
