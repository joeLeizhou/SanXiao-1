using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShaderReferenceWindow : EditorWindow
{
    private Shader _shader;
    private List<Material> _mats;
    public void Init(Shader shader, List<Material> materials)
    {
        _shader = shader;
        _mats = materials;
    }

    public void OnGUI()
    {
        if (_mats == null) return;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.ObjectField(_shader, typeof(Shader));
        for (var i = 0; i < _mats.Count; i++)
        {
            EditorGUILayout.ObjectField(_mats[i], typeof(Material));
        }
        EditorGUILayout.EndVertical();
    }
}

[CustomEditor(typeof(Shader))]
public class ShaderReferenceCheck : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUI.enabled = true;
        if (GUILayout.Button("检查依赖"))
        {
            var materials = AssetDatabase.FindAssets("t:material");
            Debug.Log("shader reference checking begin-------------");
            var mats = new List<Material>();
            for (var i = 0; i < materials.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(materials[i]);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat.shader == target)
                {
                    mats.Add(mat);
                }
            }

            var window = EditorWindow.GetWindow<ShaderReferenceWindow>();
            window.Init(target as Shader, mats);
        }
    }
}