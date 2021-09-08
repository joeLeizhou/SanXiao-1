using System;
using Orcas.Gragh;
using Orcas.Graph.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace Orcas.Gragh.Editor
{
    public sealed class JGraphPort : Port
    {
        JGraphPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            this.LoadAndAddStyleSheet("Styles/JGraphPort");
        }

        private Slot _slot;

        public static Port Create(Slot logicSlot, IEdgeConnectorListener connectorListener)
        {
            UnityEditor.Graphs.Graph graph;
            var port = new JGraphPort(Orientation.Horizontal,
                logicSlot.IsInput ? Direction.Input : Direction.Output,
                logicSlot.IsInput ? Capacity.Single : Capacity.Multi,
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.Slot = logicSlot;
            return port;
        }

        public Slot Slot
        {
            get => _slot;
            set
            {
                if (ReferenceEquals(value, _slot))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (_slot != null && value.IsInput != _slot.IsInput)
                    throw new ArgumentException("Cannot change direction of already created port");
                _slot = value;
                var typeName = Slot.Data.GetDataType().ToString();
                portName = Slot.Name + " :" + typeName;
                portColor = Slot.GetColor();
            }
        }
    }
}
