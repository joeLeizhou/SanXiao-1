using UnityEditor.Experimental.GraphView;

using UnityEngine;

namespace Orcas.Gragh.Editor
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly JGraphEditorView _logicJGraphEditorView;
        private readonly SearchWindowProvider _searchWindowProvider;

        public EdgeConnectorListener(JGraphEditorView logicJGraphEditorView, SearchWindowProvider searchWindowProvider)
        {
            _logicJGraphEditorView = logicJGraphEditorView;
            _searchWindowProvider = searchWindowProvider;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var draggedPort = (edge.output != null ? edge.output.edgeConnector.edgeDragHelper.draggedPort : null) ??
                              (edge.input != null ? edge.input.edgeConnector.edgeDragHelper.draggedPort : null);
            _searchWindowProvider.ConnectedJGraphPort = (JGraphPort)draggedPort;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                _searchWindowProvider);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            if (edge.input != null && edge.output != null)
                _logicJGraphEditorView.AddEdge(edge);
        }
    }
}