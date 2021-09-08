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
    public class VVector3 : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3 data { get; set; }
        [UnityEngine.Scripting.Preserve]
        private float3 runTimeData;
        [UnityEngine.Scripting.Preserve]
        public VVector3()
        {
        }
        [UnityEngine.Scripting.Preserve]
        public VVector3(float3 d)
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
            return VariableType.VECTOR3;
        }
        
        public override void SetVariable(object data)
        {
            runTimeData = (float3)data;
        }

        public override void SetVariable(float3 data)
        {
            runTimeData = data;
        }
        public override void SetVariable(float2 data)
        {
            runTimeData.xy = data;
        }
        public override void SetVariable(float4 data)
        {
            runTimeData = data.xyz;
        }

        public override float3 GetFloat3Variable()
        {
            return runTimeData;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }
        public override void CopyTo(IVariable variable)
        {
            ((VVector3)variable).data = this.data;
        }
#if UNITY_EDITOR
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement() { name = "inputContainer" };
            // var dummy = new VisualElement { name = "dummy" };
            // var label = new Label("X");
            // dummy.Add(label);
            // container.Add(dummy);
            var field = new Vector3Field() { value = data };
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
