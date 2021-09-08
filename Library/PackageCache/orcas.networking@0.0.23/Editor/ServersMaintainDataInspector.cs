using UnityEngine;
using Orcas.Networking;
using UnityEditor;

[CustomEditor(typeof(ServersMaintainData))]
public class ServersMaintainDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUI.enabled = false;    //禁止用户修改
         base.OnInspectorGUI();

       // EditorGUILayout.PropertyField(ServerBriefs_Prop);

        
        
        GUI.enabled = true;
       // EditorGUILayout.PropertyField(Note_Prop);
        serializedObject.ApplyModifiedProperties();


    }
}
