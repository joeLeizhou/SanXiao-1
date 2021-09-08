using System;
using Orcas.Graph;
using Orcas.Graph.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Orcas.Gragh.Editor
{
    public class PortInputView : GraphElement, IDisposable
    {
        private readonly CustomStyleProperty<Color> _edgeColorProperty = new CustomStyleProperty<Color>("--edge-color");

        public Color EdgeColor { get; private set; } = Color.red;

        public Slot Description { get; private set; }

        private VariableType _valueType;
        private VisualElement _control;
        private readonly VisualElement _container;
        private readonly EdgeControl _edgeControl;

        public PortInputView(Slot description)
        {
            this.LoadAndAddStyleSheet("Styles/PortInputView");
            pickingMode = PickingMode.Ignore;
            ClearClassList();
            Description = description;
            _valueType = description.GetDataType();
            AddToClassList("type" + _valueType);

            _edgeControl = new EdgeControl
            {
                @from = new Vector2(212f - 21f, 11.5f),
                to = new Vector2(212f, 11.5f),
                edgeWidth = 2,
                pickingMode = PickingMode.Ignore
            };
            Add(_edgeControl);

            _container = new VisualElement { name = "container" };
            {
                _control = this.Description.InstantiateControl();
                if (_control != null)
                    _container.Add(_control);

                var slotElement = new VisualElement { name = "slot" };
                {
                    slotElement.Add(new VisualElement { name = "dot" });
                }
                _container.Add(slotElement);
            }
            Add(_container);

            _container.visible = _edgeControl.visible = _control != null;
        }

        private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            if (e.customStyle.TryGetValue(_edgeColorProperty, out var colorValue))
                EdgeColor = colorValue;
            
            _edgeControl.UpdateLayout();
            _edgeControl.inputColor = EdgeColor;
            _edgeControl.outputColor = EdgeColor;
        }

        public void UpdateSlot(Slot newLogicSlot)
        {
            Description = newLogicSlot;
            Recreate();
        }

        public void UpdateSlotType()
        {
            if (Description.GetDataType() != _valueType)
                Recreate();
        }

        void Recreate()
        {
            RemoveFromClassList("type" + _valueType);
            _valueType = Description.GetDataType();
            AddToClassList("type" + _valueType);
            if (_control != null)
            {
                var disposable = _control as IDisposable;
                disposable?.Dispose();
                _container.Remove(_control);
            }
            _control = Description.InstantiateControl();
            if (_control != null)
                _container.Insert(0, _control);

            _container.visible = _edgeControl.visible = _control != null;
        }

        public void Dispose()
        {
            var disposable = _control as IDisposable;
            disposable?.Dispose();
        }
    }
}
