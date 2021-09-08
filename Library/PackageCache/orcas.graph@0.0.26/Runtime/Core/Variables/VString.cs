using Newtonsoft.Json;
using System;
using Orcas.Graph.Core;
#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif

namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VString : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        public string data { get; set; } = "";
        [UnityEngine.Scripting.Preserve]
        private string runTimeData;
        [UnityEngine.Scripting.Preserve]
        public VString()
        {

        }
        [UnityEngine.Scripting.Preserve]
        public VString(string s)
        {
            data = s;
        }
        public override bool CheckCastDataType(VariableType dataType)
        {
            return dataType == VariableType.STRING||
                   dataType == VariableType.OBJECT;
        }
        public override void ResetVariable()
        {
            if (data != null)
                runTimeData = data;
            else
                runTimeData = string.Empty;
        }

        public override VariableType GetDataType()
        {
            return VariableType.STRING;
        }

        public override void SetVariable(string data)
        {
            if (data != null)
            {
                runTimeData = data;
            }
            else
            {
                runTimeData = string.Empty;
            }
        }
        public override void SetVariable(object data)
        {
            if (data != null)
            {
                runTimeData = data.ToString();
            }
            else
            {
                runTimeData = string.Empty;
            }
        }

        public override string GetStringVariable()
        {
            return runTimeData;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }
        public override void CopyTo(IVariable variable)
        {
            ((VString)variable).data = this.data;
        }
#if UNITY_EDITOR
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement() { name = "inputContainer" };
            var dummy = new VisualElement { name = "dummy" };
            var label = new Label("X");
            dummy.Add(label);
            container.Add(dummy);

            var field = new TextField() { value = data };
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
