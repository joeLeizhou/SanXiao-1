using Newtonsoft.Json;
using Orcas.Graph.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Orcas.Graph.Variables
{
    public class VColor : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        public Color data { get; set; }
        [UnityEngine.Scripting.Preserve]
        private float4 runTimeData;
        public override bool CheckCastDataType(VariableType dataType)
        {
            return dataType == VariableType.VECTOR2 || dataType == VariableType.VECTOR3 || dataType == VariableType.COLOR || dataType == VariableType.OBJECT;
        }

        public override void ResetVariable()
        {
            runTimeData = new float4(data.r, data.g, data.b, data.a);
        }

        public override VariableType GetDataType()
        {
            return VariableType.COLOR;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }

        public override void SetVariable(float4 data)
        {
            runTimeData = data;
        }

        public override void SetVariable(float3 data)
        {
            runTimeData.xyz = data;
        }

        public override void SetVariable(float2 data)
        {
            runTimeData.xy = data;
        }
        
        public override void SetVariable(object data)
        {
            runTimeData = (float4) data;
        }

        public override float4 GetColorVariable()
        {
            return runTimeData;
        }
        public override void CopyTo(IVariable variable)
        {
            ((VColor)variable).data = this.data;
        }
    }
}
