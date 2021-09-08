using Newtonsoft.Json;
using System;
using Orcas.Graph.Core;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VVector2 : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        public Vector2 data { get; set; }
        [UnityEngine.Scripting.Preserve]
        private float2 runTimeData;
        [UnityEngine.Scripting.Preserve]
        public VVector2()
        {
        }
        [UnityEngine.Scripting.Preserve]
        public VVector2(float2 d)
        {
            data = d;
        }

        public override bool CheckCastDataType(VariableType dataType)
        {
            return dataType == VariableType.VECTOR2 || dataType == VariableType.VECTOR3 ||
                   dataType == VariableType.COLOR ||
                   dataType == VariableType.OBJECT;
        }

        public override void ResetVariable()
        {
            runTimeData = data;
        }

        public override VariableType GetDataType()
        {
            return VariableType.VECTOR2;
        }
        
        public override void SetVariable(object data)
        {
            runTimeData = (float2)data;
        }

        public override void SetVariable(float2 data)
        {
            runTimeData = data;
        }
        public override void SetVariable(float3 data)
        {
            runTimeData = data.xy;
        }
        public override void SetVariable(float4 data)
        {
            runTimeData = data.xy;
        }

        public override float2 GetFloat2Variable()
        {
            return runTimeData;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }
        public override void CopyTo(IVariable variable)
        {
            ((VVector2)variable).data = this.data;
        }
#if UNITY_EDITOR
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement() { name = "inputContainer" };
            // var dummy = new VisualElement { name = "dummy" };
            // var label = new Label("X");
            // dummy.Add(label);
            // container.Add(dummy);
            var field = new Vector2Field() { value = data };
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
