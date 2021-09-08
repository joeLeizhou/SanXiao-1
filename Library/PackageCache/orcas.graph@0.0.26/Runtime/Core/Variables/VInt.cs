using Newtonsoft.Json;
using System;
using Orcas.Graph.Core;
#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VInt : IVariable
    {
        [UnityEngine.Scripting.Preserve] public int data { get; set; }
        [UnityEngine.Scripting.Preserve] private int runTimeData;

        [UnityEngine.Scripting.Preserve]
        public VInt()
        {
        }

        [UnityEngine.Scripting.Preserve]
        public VInt(int d)
        {
            data = d;
        }

        public override bool CheckCastDataType(VariableType dataType)
        {
            return dataType == VariableType.FLOAT || dataType == VariableType.INT ||
                   dataType == VariableType.LONG || dataType == VariableType.BOOL ||
                   dataType == VariableType.OBJECT;
        }

        public override void ResetVariable()
        {
            runTimeData = data;
        }

        public override VariableType GetDataType()
        {
            return VariableType.INT;
        }

        public override void SetVariable(object data)
        {
            runTimeData = (int) data;
        }

        public override void SetVariable(int data)
        {
            runTimeData = data;
        }

        public override void SetVariable(float data)
        {
            runTimeData = (int) data;
        }

        public override void SetVariable(bool data)
        {
            runTimeData = data ? 1 : 0;
        }

        public override int GetIntVariable()
        {
            return runTimeData;
        }

        public override bool GetBoolVariable()
        {
            return runTimeData > 0;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }

        public override void CopyTo(IVariable variable)
        {
            ((VInt) variable).data = this.data;
        }
#if UNITY_EDITOR
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement() {name = "inputContainer"};
            var dummy = new VisualElement {name = "dummy"};
            var label = new Label("X");
            dummy.Add(label);
            container.Add(dummy);

            var field = new IntegerField() {value = data};
            field.RegisterValueChangedCallback(evt => { data = evt.newValue; });
            container.Add(field);
            return container;
        }
#endif
    }
}
