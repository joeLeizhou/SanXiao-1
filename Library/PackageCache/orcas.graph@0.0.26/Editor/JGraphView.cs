using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

namespace Orcas.Gragh.Editor
{
    /// <summary>
    /// Implementation of GraphView
    /// </summary>
    public class JGraphView : GraphView
    {
        public Graph.Core.Graph Graph { get; private set; }
        public Vector2 MousePosition;

        public JGraphView()
        {
            this.LoadAndAddStyleSheet("Styles/JGraphView");
        }

        public JGraphView(Graph.Core.Graph logicGraphEditorObject) : this()
        {
            Graph = logicGraphEditorObject;
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatibleAnchors = new List<Port>();
            var startSlot = (startAnchor as JGraphPort)?.Slot;
            if (startSlot == null)
                return compatibleAnchors;

            compatibleAnchors.AddRange(from candidateAnchor in ports.ToList() let candidateSlot = (candidateAnchor as JGraphPort)?.Slot where startSlot.IsCompatibleWith(candidateSlot) select candidateAnchor);
            return compatibleAnchors;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            MousePosition = evt.mousePosition;
        }

    }
}