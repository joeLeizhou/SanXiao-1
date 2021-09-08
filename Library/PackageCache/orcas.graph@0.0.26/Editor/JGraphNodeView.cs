using System.Collections.Generic;
using System.Linq;
using Orcas.Graph.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

using UnityEngine.UIElements;

namespace Orcas.Gragh.Editor
{
    /// <summary>
    /// Actual visual nodes which gets added to the graph UI.
    /// </summary>
    public class JGraphNodeView : Node
    {
        private VisualElement _controlsDivider;
        private VisualElement _controlItems;
        private VisualElement _portInputContainer;
        private IEdgeConnectorListener _connectorListener;

        public ModuleBase LogicNode { get; private set; }

        public void Initialize(ModuleBase logicNodeEditor, IEdgeConnectorListener connectorListener)
        {
            this.LoadAndAddStyleSheet("Styles/JGraphNodeView");

            if (logicNodeEditor.Outputs.Length == 0)
                AddToClassList("input");
            else if (logicNodeEditor.Inputs.Length == 0)
                AddToClassList("output");

            _connectorListener = connectorListener;
            LogicNode = logicNodeEditor;
            title = LogicNode.Name + "(" + LogicNode.ID + ")";

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement { name = "controls" };
            {
                _controlsDivider = new VisualElement { name = "divider" };
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement { name = "items" };
                controlsContainer.Add(_controlItems);

                // foreach (var propertyInfo in
                //     logicNodeEditor.GetType().GetProperties(BindingFlags.Instance |
                //                                             BindingFlags.Public |
                //                                             BindingFlags.NonPublic))
                // {
                //     // foreach (INodeControlAttribute attribute in
                //     //     propertyInfo.GetCustomAttributes(typeof(INodeControlAttribute), false))
                //     // {
                //     //     _controlItems.Add(attribute.InstantiateControl(logicNodeEditor, propertyInfo));
                //     // }
                // }
            }
            contents.Add(controlsContainer);

            // Add port input container, which acts as a pixel cache for all port inputs
            _portInputContainer = new VisualElement
            {
                name = "portInputContainer",
                //                clippingOptions = ClippingOptions.ClipAndCacheContents,
                pickingMode = PickingMode.Ignore
            };
            Add(_portInputContainer);

            List<Slot> foundSlots = new List<Slot>();
            logicNodeEditor.GetSlots(foundSlots);
            AddPorts(foundSlots);

            SetPosition(logicNodeEditor.Rect);
            UpdatePortInputs();
            base.expanded = true;
            RefreshExpandedState();
            UpdatePortInputVisibilities();
        }

        private void AddPorts(IEnumerable<Slot> slots)
        {
            foreach (var slot in slots)
            {
                var port = JGraphPort.Create(slot, _connectorListener);
                if (slot.IsInput)
                    inputContainer.Add(port);
                else
                    outputContainer.Add(port);
            }
        }

        public override bool expanded
        {
            get { return base.expanded; }
            set
            {
                Debug.Log(value);
                if (base.expanded != value)
                    base.expanded = value;

                RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
                UpdatePortInputVisibilities();
            }
        }

        void UpdatePortInputs()
        {
            foreach (var port in inputContainer.Children().OfType<JGraphPort>())
            {
                if (port.Slot.LinkedSlots.Count > 0)
                    continue;
                if (_portInputContainer.Children().OfType<PortInputView>().Any(a => Equals(a.Description, port.Slot)))
                    continue;
                {
                    var portInputView = new PortInputView(port.Slot) { style = { position = Position.Absolute } };
                    if (portInputView.visible == false)
                        continue;
                    _portInputContainer.Add(portInputView);
                    if (float.IsNaN(port.layout.width))
                    {
                        port.RegisterCallback<GeometryChangedEvent>(UpdatePortInput);
                    }
                    else
                    {
                        SetPortInputPosition(port, portInputView);
                    }
                }
            }
        }

        void SetPortInputPosition(JGraphPort port, PortInputView inputView)
        {
            inputView.style.top = port.layout.y;
            inputView.parent.style.height = inputContainer.layout.height;
        }


        void UpdatePortInput(GeometryChangedEvent evt)
        {
            var port = (JGraphPort)evt.target;
            var inputView = _portInputContainer.Children().OfType<PortInputView>().First(x => Equals(x.Description, port.Slot));

            SetPortInputPosition(port, inputView);
            port.UnregisterCallback<GeometryChangedEvent>(UpdatePortInput);
        }

        public void UpdatePortInputVisibilities()
        {
            foreach (var portInputView in _portInputContainer.Children().OfType<PortInputView>())
            {
                var slot = portInputView.Description;
                var oldVisibility = portInputView.visible;
                portInputView.visible = expanded && slot.LinkedSlots.Count == 0;
                portInputView.style.display = portInputView.visible ? DisplayStyle.Flex : DisplayStyle.None;
                if (portInputView.visible != oldVisibility)
                    _portInputContainer.MarkDirtyRepaint();
            }
        }
    }
}