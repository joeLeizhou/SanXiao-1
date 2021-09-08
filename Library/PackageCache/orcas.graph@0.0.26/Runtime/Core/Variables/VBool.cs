using Newtonsoft.Json;
using System;
using Orcas.Graph.Core;
#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif

namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VBool : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        public bool data { get; set; }
        [UnityEngine.Scripting.Preserve]
        private bool runTimeData;
        [UnityEngine.Scripting.Preserve]
        public VBool()
        {
        }
        [UnityEngine.Scripting.Preserve]
        public VBool(bool d)
        {
            data = d;
        }
        public override bool CheckCastDataType(VariableType dataType)
        {
            return dataType == VariableType.INT || dataType == VariableType.LONG || dataType == VariableType.BOOL || dataType == VariableType.OBJECT;
        }

        public override void ResetVariable()
        {
            runTimeData = data;
        }

        public override VariableType GetDataType()
        {
            return VariableType.BOOL;
        }

        public override void SetVariable(object data)
        {
            runTimeData = (bool) data;
        }

        public override void SetVariable(bool data)
        {
            runTimeData = data;
        }
        public override void SetVariable(int data)
        {
            runTimeData = data > 0;
        }

        public override bool GetBoolVariable()
        {
            return runTimeData;
        }
        public override int GetIntVariable()
        {
            return runTimeData ? 1 : 0;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }

        public override void CopyTo(IVariable variable)
        {
            ((VBool)variable).data = this.data;
        }
#if UNITY_EDITOR
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement() { name = "inputContainer" };
            var dummy = new VisualElement { name = "dummy" };
            var label = new Label("X");
            dummy.Add(label);
            container.Add(dummy);

            var field = new Toggle() { value = data };
            field.RegisterValueChangedCallback(evt =>
            {
                data = evt.newValue;
            });
            container.Add(field);
            return container;
        }
#endif
    }
}
