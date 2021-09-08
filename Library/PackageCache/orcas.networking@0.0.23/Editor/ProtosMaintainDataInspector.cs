using UnityEngine;
using Orcas.Networking;
using UnityEditor;

[CustomEditor(typeof(ProtosMaintainData))]
public class ProtosMaintainDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUI.enabled = false;    //禁止用户修改
        base.OnInspectorGUI();
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();
    }
}
