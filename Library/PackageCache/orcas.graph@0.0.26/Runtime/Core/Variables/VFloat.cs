using Newtonsoft.Json;
using Orcas.Graph.Core;
#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif
namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VFloat : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        public float data { get; set; }
        [UnityEngine.Scripting.Preserve]
        private float runTimeData;
        [UnityEngine.Scripting.Preserve]
        public VFloat()
        {
        }
        [UnityEngine.Scripting.Preserve]
        public VFloat(float d)
        {
            data = d;
        }
        public override bool CheckCastDataType(VariableType dataType)
        {
            return dataType == VariableType.FLOAT || dataType == VariableType.INT || dataType == VariableType.LONG || dataType == VariableType.OBJECT;
        }

        public override void ResetVariable()
        {
            runTimeData = data;
        }

        public override VariableType GetDataType()
        {
            return VariableType.FLOAT;
        }

        public override void SetVariable(object data)
        {
            runTimeData = (float) data;
        }

        public override void SetVariable(float data)
        {
            runTimeData = data;
        }
        public override void SetVariable(int data)
        {
            runTimeData = (float)data;
        }
        public override float GetFloatVariable()
        {
            return runTimeData;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }
        public override void CopyTo(IVariable variable)
        {
            ((VFloat)variable).data = this.data;
        }
#if UNITY_EDITOR
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement() { name = "inputContainer" };
            var dummy = new VisualElement { name = "dummy" };
            var label = new Label("X");
            dummy.Add(label);
            container.Add(dummy);
            var field = new FloatField() { value = data };
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
