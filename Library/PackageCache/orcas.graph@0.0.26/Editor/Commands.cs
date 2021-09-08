using System.IO;
using UnityEditor;
using UnityEngine;

namespace Orcas.Gragh.Editor
{
    public static class Commands
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/工具/图")]
        public static void CreateBlackboard()
        {
            var window = EditorWindow.GetWindow<JGraphEditorWindow>();
            var graph = ScriptableObject.CreateInstance<Graph.Core.Graph>();
            if (Selection.activeObject == null) return;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            // if (!path.Contains("Assets/Resources"))
            // {
            //     Debug.LogError("只能在resources目录下创建图!");
            //     return;
            // }

            //path = path.Substring("Assets/Resources".Length);
            graph.SetPath(Path.Combine(path, "New Graph.asset"));
            window.Initialize(graph, "New Graph");

            Selection.activeObject = window;
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string guid = "";
            long localid = 0;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceID, out guid, out localid);
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj.GetType() != typeof(Graph.Core.Graph)) return false;
            var assetPath = AssetDatabase.GetAssetPath(obj);

            var graph = Graph.Core.Graph.Load(Path.Combine(Path.GetDirectoryName(assetPath) + "/", Path.GetFileNameWithoutExtension(assetPath)));
            var objGraph = obj as Graph.Core.Graph;
            graph.SetPath(assetPath);
            objGraph.CopyFrom(graph);
            var window = EditorWindow.GetWindow<JGraphEditorWindow>();
            window.Initialize(objGraph, guid);
            return true;
        }
#endif
    }
}
