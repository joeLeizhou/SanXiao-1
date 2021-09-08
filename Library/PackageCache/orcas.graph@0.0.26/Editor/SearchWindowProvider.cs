using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orcas.Graph.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

namespace Orcas.Gragh.Editor
{
    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private JGraphEditorWindow _editorWindow;
        private JGraphEditorView _logicJGraphEditorView;
        private JGraphView _graphView;
        private Texture2D _icon;
        public JGraphPort ConnectedJGraphPort { get; set; }
        public bool NodeNeedsRepositioning { get; set; }
        public Vector2 TargetPosition { get; private set; }

        public void Initialize(JGraphEditorWindow editorWindow,
            JGraphEditorView logicJGraphEditorView,
            JGraphView graphView)
        {
            _editorWindow = editorWindow;
            _logicJGraphEditorView = logicJGraphEditorView;
            _graphView = graphView;

            // Transparent icon to trick search window into indenting items
            _icon = new Texture2D(1, 1);
            _icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _icon.Apply();
        }

        void OnDestroy()
        {
            if (_icon != null)
            {
                DestroyImmediate(_icon);
                _icon = null;
            }
        }

        struct NodeEntry
        {
            public int Order;
            public string[] Title;
            public Type NodeType;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var nodeEntries = new List<NodeEntry>();
            {
                // First build up temporary data structure containing group & title as an array of strings (the last one is the actual title) and associated node type.
                // foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    // foreach (var type in GetTypesOrNothing(assembly))
                    foreach (var type in TypeCache.GetTypesWithAttribute<GraphModuleAttribute>())
                    {
                        if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ModuleBase)))
                        {
                            var attrs = type.GetCustomAttributes(typeof(GraphModuleAttribute), false) as GraphModuleAttribute[];
                            if (attrs != null && attrs.Length > 0)
                            {
                                AddEntries(type, attrs[0].Title, attrs[0].Order, nodeEntries);
                            }
                        }
                    }
                }
                // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group.
                // Example result:
                // - Art/BlendMode
                // - Art/Adjustments/ColorBalance
                // - Art/Adjustments/Contrast
                nodeEntries.Sort((entry1, entry2) =>
                {
                    for (var i = 0; i < entry1.Title.Length; i++)
                    {
                        if (i >= entry2.Title.Length)
                            return 1;
                        var value = entry1.Title[i].CompareTo(entry2.Title[i]);
                        if (value != 0)
                        {
                            // Set order in last level
                            if (entry1.Order != entry2.Order && i == entry1.Title.Length - 1 && i == entry2.Title.Length - 1)
                                return entry1.Order < entry2.Order ? -1 : 1;
                            // Make sure that leaves go before nodes
                            if (entry1.Title.Length != entry2.Title.Length && (i == entry1.Title.Length - 1 || i == entry2.Title.Length - 1))
                                return entry1.Title.Length < entry2.Title.Length ? -1 : 1;
                            return value;
                        }
                    }
                    return 0;
                });
            }

            //* Build up the data structure needed by SearchWindow.

            // `groups` contains the current group path we're in.
            var groups = new List<string>();

            // First item in the tree is the title of the window.
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            foreach (var nodeEntry in nodeEntries)
            {
                // `createIndex` represents from where we should add new group entries from the current entry's group path.
                var createIndex = int.MaxValue;

                // Compare the group path of the current entry to the current group path.
                for (var i = 0; i < nodeEntry.Title.Length - 1; i++)
                {
                    if (i >= groups.Count)
                    {
                        // The current group path matches a prefix of the current entry's group path, so we add the
                        // rest of the group path from the currrent entry.
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != nodeEntry.Title[i])
                    {
                        // A prefix of the current group path matches a prefix of the current entry's group path,
                        // so we remove everyfrom from the point where it doesn't match anymore, and then add the rest
                        // of the group path from the current entry.
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                // Create new group entries as needed.
                // If we don't need to modify the group path, `createIndex` will be `int.MaxValue` and thus the loop won't run.
                for (var i = createIndex; i < nodeEntry.Title.Length - 1; i++)
                {
                    var group = nodeEntry.Title[i];
                    groups.Add(group);
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                // Finally, add the actual entry.
                tree.Add(new SearchTreeEntry(new GUIContent(string.Join(".", nodeEntry.Title), _icon)) { level = nodeEntry.Title.Length, userData = nodeEntry });
            }

            return tree;
        }

        public static IEnumerable<Type> GetTypesOrNothing(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        void AddEntries(Type NodeType, string[] title, int order, List<NodeEntry> nodeEntries)
        {
            // if (ConnectedLogicPort == null)
            {
                nodeEntries.Add(new NodeEntry
                {
                    NodeType = NodeType,
                    Title = title,
                    Order = order,
                });
                return;
            }
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var nodeEntry = (NodeEntry)entry.userData;
            var nodeEditor = nodeEntry.NodeType;

            var windowMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent, context.screenMousePosition - _editorWindow.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);
            var position = new Vector3(graphMousePosition.x, graphMousePosition.y, 0);

            _logicJGraphEditorView.AddNode(nodeEditor, position);

            return true;
        }
    }
}
