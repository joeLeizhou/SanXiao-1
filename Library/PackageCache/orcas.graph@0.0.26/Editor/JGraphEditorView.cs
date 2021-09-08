using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Orcas.Graph.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

namespace Orcas.Gragh.Editor
{
    /// <summary>
    /// Root Visual element which takes up entire editor window. Every other visual element is a child
    /// of this.
    /// </summary>
    public class JGraphEditorView : VisualElement
    {
        private readonly Graph.Core.Graph _logicGraph;
        private readonly JGraphView _graphView;
        private JGraphEditorWindow _editorWindow;
        private readonly EdgeConnectorListener _edgeConnectorListener;
        private readonly SearchWindowProvider _searchWindowProvider;

        public Action saveRequested { get; set; }

        public Action showInProjectRequested { get; set; }

        public JGraphView JGraphView => _graphView;

        public JGraphEditorView(JGraphEditorWindow editorWindow, Graph.Core.Graph logicGraphEditorObject)
        {
            Debug.Log(logicGraphEditorObject.GetInstanceID());
            _editorWindow = editorWindow;
            _logicGraph = logicGraphEditorObject;
            // _logicGraphEditorObject.Deserialized += LogicGraphEditorDataOnDeserialized;

            this.LoadAndAddStyleSheet("Styles/JGraphEditorView");

            var toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                {
                    saveRequested?.Invoke();
                }

                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                    showInProjectRequested?.Invoke();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            });
            Add(toolbar);

            var content = new VisualElement { name = "content" };
            {
                _graphView = new JGraphView(_logicGraph)
                {
                    name = "GraphView",
                };

                _graphView.SetupZoom(0.2f, ContentZoomer.DefaultMaxScale + 0.5f);
                _graphView.AddManipulator(new ContentDragger());
                _graphView.AddManipulator(new SelectionDragger());
                _graphView.AddManipulator(new RectangleSelector());
                _graphView.AddManipulator(new ClickSelector());
                _graphView.RegisterCallback<KeyDownEvent>(KeyDown);
                content.Add(_graphView);

                _graphView.graphViewChanged = OnGraphViewChanged;
                _graphView.serializeGraphElements = OnSerializeGraphElements;
                _graphView.canPasteSerializedData = OnCanPasteSerializedData;
                _graphView.unserializeAndPaste = OnUnserializeAndPaste;
            }

            _searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            _searchWindowProvider.Initialize(editorWindow, this, _graphView);

            _edgeConnectorListener = new EdgeConnectorListener(this, _searchWindowProvider);
            _graphView.nodeCreationRequest = (c) =>
            {
                _searchWindowProvider.ConnectedJGraphPort = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindowProvider);
            };

            LoadElements();

            Add(content);
        }

        private void LoadElements()
        {
            for (var i = 0; i < _logicGraph.Modules.Count; i++)
            {
                AddNode(_logicGraph.Modules[i]);
            }
            for (var i = 0; i < _logicGraph.SerializedEdges.Count; i++)
            {
                AddEdge(_logicGraph.SerializedEdges[i]);
            }
        }

        public void HandleGraphChanges()
        {
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            Debug.Log($"GraphViewChanged {graphViewChange}");

            if (graphViewChange.edgesToCreate != null)
                Debug.Log("EDGES TO CREATE " + graphViewChange.edgesToCreate.Count);

            if (graphViewChange.movedElements != null)
            {
                Debug.Log("Moved elements " + graphViewChange.movedElements.Count);
                _logicGraph.RegisterCompleteObjectUndo("Graph Element Moved.");
                foreach (var nodeView in graphViewChange.movedElements.OfType<JGraphNodeView>())
                {
                    nodeView.LogicNode.Rect = nodeView.GetPosition();
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                Debug.Log("Elements to remove" + graphViewChange.elementsToRemove.Count);
                _logicGraph.RegisterCompleteObjectUndo("Deleted Graph Elements.");

                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    var serializedEdge = edge.userData as SerializedEdge;
                    _logicGraph.RemoveEdge(serializedEdge);
                    UpdatePortINputByNodeID(serializedEdge.TargetNodeGuid);
                }

                foreach (var nodeView in graphViewChange.elementsToRemove.OfType<JGraphNodeView>())
                {
                    _logicGraph.RemoveModule(nodeView.LogicNode.ID);
                }

            }

            return graphViewChange;
        }

        private const string k_SkillCopyMark = "skill_modole_copy_";
        private string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            List<object> datas = new List<object>();
            foreach (var element in elements)
            {
                if (element is JGraphNodeView)
                {
                    datas.Add((element as JGraphNodeView).userData);
                }
            }
            if (datas.Count == 0)
                return "";
            var dataStr = k_SkillCopyMark + JsonConvert.SerializeObject(datas);
            Debug.Log("copy data " + dataStr);
            return dataStr;
        }

        private bool OnCanPasteSerializedData(string data)
        {
            if (string.IsNullOrEmpty(data) || data.StartsWith(k_SkillCopyMark) == false)
                return false;
            // Debug.Log("can copy " + data);
            return true;
        }

        private void OnUnserializeAndPaste(string operationName, string data)
        {
            if (string.IsNullOrEmpty(data) || data.StartsWith(k_SkillCopyMark) == false)
                return;
            data = data.Substring(k_SkillCopyMark.Length);
            var serializedNodes = JsonConvert.DeserializeObject<List<SerializedNode>>(data);
            var isDuplicte = operationName == "Duplicate";
            for (int i = 0; i < serializedNodes.Count; i++)
            {
                var moduleType = Type.GetType(serializedNodes[i].NodeType + ", Assembly-CSharp");
                if (moduleType == null)
                    continue;

                var sourceModule = _logicGraph.GetModuleById(serializedNodes[i].NodeGuid);
                var logicNodeEditor = _logicGraph.CreateModule(moduleType);
                var rect = new Rect(Vector2.zero, Vector2.one * 50);
                if (sourceModule != null)
                {
                    rect.position = sourceModule.Rect.position + new Vector2(50, 50);
                    sourceModule.CopyDatasTo(logicNodeEditor);
                }
                logicNodeEditor.Rect = rect;
                AddNode(logicNodeEditor);
            }
            // Debug.Log("plaste " + operationName + " ," + data);
        }

        private void UpdatePortINputByNodeID(int id)
        {
            JGraphNodeView targetNodeView = _graphView.nodes.ToList().OfType<JGraphNodeView>()
                                               .FirstOrDefault(x => x.LogicNode.ID == id);
            if (targetNodeView == null)
            {
                Debug.LogWarning($"Target NodeGUID not found {id}");
            }
            else
            {
                targetNodeView.UpdatePortInputVisibilities();
            }
        }

        private void KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Space && !evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey)
            {
            }
            else if (evt.keyCode == KeyCode.F1)
            {
            }
            else if (evt.ctrlKey && evt.keyCode == KeyCode.S)
            {
                saveRequested?.Invoke();
            }
        }

        public SerializedNode AddNode(Type moduleType, Vector2 pos)
        {
            var logicNodeEditor = _logicGraph.CreateModule(moduleType);
            logicNodeEditor.Rect = new Rect(pos, Vector2.one * 50);
            return AddNode(logicNodeEditor);
        }
        public SerializedNode AddNode(ModuleBase logicNodeEditor)
        {
            _logicGraph.RegisterCompleteObjectUndo("Add Node " + logicNodeEditor.Name);

            SerializedNode serializedNode = new SerializedNode
            {
                NodeGuid = logicNodeEditor.ID,
                NodeType = logicNodeEditor.GetType().ToString(),
            };

            var nodeView = new JGraphNodeView { userData = serializedNode };
            _graphView.AddElement(nodeView);
            nodeView.Initialize(logicNodeEditor, _edgeConnectorListener);
            nodeView.MarkDirtyRepaint();
            return serializedNode;
        }

        public void RemoveEdgesConnectedTo(JGraphPort jGraphPort)
        {
            _foundEdges.Clear();
            _logicGraph.GetEdges(jGraphPort.Slot.ModId, jGraphPort.Slot.ID, _foundEdges);
            for (int i = 0; i < _foundEdges.Count; ++i)
            {
                RemoveEdge(_foundEdges[i]);
            }
        }

        private readonly List<SerializedEdge> _foundEdges = new List<SerializedEdge>();

        public void RemoveEdge(SerializedEdge serializedEdge)
        {
            _logicGraph.RemoveEdge(serializedEdge);
            foreach (var edge in _graphView.edges.ToList())
            {
                if (edge.userData == serializedEdge)
                {
                    Debug.Log("removing edge " + edge);
                    _graphView.RemoveElement(edge);
                }
            }
        }

        public void AddEdge(Edge edgeView)
        {
            Slot leftLogicSlot;
            Slot rightLogicSlot;
            GetSlots(edgeView, out leftLogicSlot, out rightLogicSlot);
            if (leftLogicSlot.IsInput == rightLogicSlot.IsInput)
            {
                Debug.LogWarning("Both input/output slot!!!");
                return;
            }
            if (rightLogicSlot.IsCompatibleWith(leftLogicSlot) == false)
            {
                Debug.LogWarning("input/output slot data type not match!!!");
                return;
            }
            if (rightLogicSlot.IsInput && rightLogicSlot.LinkedSlots.Count > 0)
            {
                Debug.LogWarning("input already connected!!!");
                var fineEdge = _logicGraph.FindEdge(rightLogicSlot.LinkedSlots[0], rightLogicSlot.ID);
                if (fineEdge != null)
                    RemoveEdge(fineEdge);
            }

            _logicGraph.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                SourceNodeGuid = leftLogicSlot.ModId,
                SourceSlotGuid = leftLogicSlot.ID,
                TargetNodeGuid = rightLogicSlot.ModId,
                TargetSlotGuid = rightLogicSlot.ID
            };

            if (_logicGraph.AddEdge(leftLogicSlot, rightLogicSlot, serializedEdge) == false)
            {
                Debug.LogWarning("already added to graph!!!");
                return;
            }

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);

            UpdatePortINputByNodeID(serializedEdge.SourceNodeGuid);
            UpdatePortINputByNodeID(serializedEdge.TargetNodeGuid);
        }

        public bool AddEdge(SerializedEdge serializedEdge)
        {
            JGraphNodeView sourceNodeView = _graphView.nodes.ToList().OfType<JGraphNodeView>()
                .FirstOrDefault(x => x.LogicNode.ID == serializedEdge.SourceNodeGuid);
            if (sourceNodeView == null)
            {
                Debug.LogWarning($"Source NodeGUID not found {serializedEdge.SourceNodeGuid}");
                return false;
            }

            JGraphPort sourceAnchor = sourceNodeView.outputContainer.Children().OfType<JGraphPort>()
                .FirstOrDefault(x => x.Slot.ID == serializedEdge.SourceSlotGuid);
            if (sourceAnchor == null)
            {
                Debug.LogError($"Source anchor null {serializedEdge.SourceSlotGuid} {serializedEdge.SourceNodeGuid}");
                return false;
            }

            JGraphNodeView targetNodeView = _graphView.nodes.ToList().OfType<JGraphNodeView>()
                .FirstOrDefault(x => x.LogicNode.ID == serializedEdge.TargetNodeGuid);
            if (targetNodeView == null)
            {
                Debug.LogWarning($"Target NodeGUID not found {serializedEdge.TargetNodeGuid}");
                return false;
            }

            JGraphPort targetAnchor = targetNodeView.inputContainer.Children().OfType<JGraphPort>()
                .FirstOrDefault(x => x.Slot.ID == serializedEdge.TargetSlotGuid);
            if (targetAnchor == null)
            {
                Debug.LogError($"Target anchor null {serializedEdge.SourceSlotGuid} {serializedEdge.TargetNodeGuid}");
                return false;
            }

            var edgeView = new Edge
            {
                userData = serializedEdge,
                output = sourceAnchor,
                input = targetAnchor
            };

            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
            targetNodeView.UpdatePortInputVisibilities();
            sourceNodeView.UpdatePortInputVisibilities();

            return true;
        }


        private void GetSlots(Edge edge, out Slot leftLogicSlot,
            out Slot rightLogicSlot)
        {
            leftLogicSlot = (edge.output as JGraphPort)?.Slot;
            rightLogicSlot = (edge.input as JGraphPort)?.Slot;
            if (leftLogicSlot == null || rightLogicSlot == null)
            {
                Debug.Log("an edge is null");
            }
        }
    }
}