using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Orcas.Gragh.Editor
{
    /// <summary>
    /// Root Editor Window. Entire system is a subset of this class.
    /// </summary>
    public class JGraphEditorWindow : EditorWindow
    {
        private Graph.Core.Graph _logicGraph;

        private JGraphEditorView _jGraphEditorView;
        private string _selectedGuid;

        private JGraphEditorView JGraphEditorView
        {
            get { return _jGraphEditorView; }
            set
            {
                if (_jGraphEditorView != null)
                {
                    _jGraphEditorView.RemoveFromHierarchy();
                }

                _jGraphEditorView = value;

                if (_jGraphEditorView != null)
                {
                    _jGraphEditorView.saveRequested += UpdateAsset;
                    _jGraphEditorView.showInProjectRequested += PingAsset;
                    this.rootVisualElement.Add(_jGraphEditorView);
                }
            }
        }

        public string SelectedGuid
        {
            get { return _selectedGuid; }
        }

        public void Initialize(Graph.Core.Graph graph, string guid)
        {
            try
            {
                _selectedGuid = guid;
                // var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                // var path = AssetDatabase.GetAssetPath(asset);
                // var textGraph = File.ReadAllText(path, Encoding.UTF8);

                _logicGraph = graph;
                // LogicGraphData logicGraphData = JsonUtility.FromJson<LogicGraphData>(textGraph);
                // _logicGraphEditorObject.Initialize(logicGraphData);
                JGraphEditorView = new JGraphEditorView(this, _logicGraph);
                JGraphEditorView.RegisterCallback<GeometryChangedEvent>(OnPostLayout);

                titleContent = new GUIContent(_logicGraph.name);

                Repaint();
            }
            catch (Exception)
            {
                _jGraphEditorView = null;
                _logicGraph = null;
                throw;
            }
        }

        private void OnDisable()
        {
            JGraphEditorView = null;
        }

        private void OnDestroy()
        {
            if (string.IsNullOrWhiteSpace(_selectedGuid) && _logicGraph != null && _logicGraph.Modules.Count > 0)
            {
                var path = EditorUtility.SaveFilePanelInProject("save skill graph", "New Graph", "asset", "Graph", _logicGraph.GetPath());
                Debug.Log("save path " + path);
                if (string.IsNullOrEmpty(path) == false)
                {
                    _logicGraph.SetPath(path);
                    _logicGraph.Save();
                }
            }
            JGraphEditorView = null;
        }

        void Update()
        {
            JGraphEditorView.HandleGraphChanges();
        }

        public void PingAsset()
        {
            if (_selectedGuid == null) return;
            var path = AssetDatabase.GUIDToAssetPath(_selectedGuid);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            EditorGUIUtility.PingObject(asset);
        }

        public void UpdateAsset()
        {
            if (SelectedGuid != null && _logicGraph != null)
            {
                _logicGraph.Save();
            }
        }

        void OnPostLayout(GeometryChangedEvent evt)
        {
            JGraphEditorView.UnregisterCallback<GeometryChangedEvent>(OnPostLayout);
            JGraphEditorView.JGraphView.FrameAll();
        }
    }
}