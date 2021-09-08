using Newtonsoft.Json;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VObject : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        private object runTimeData;
        public override bool CheckCastDataType(VariableType dataType)
        {
            return true;
        }


        public override void ResetVariable()
        {
            runTimeData = null;
        }

        public override VariableType GetDataType()
        {
            return VariableType.OBJECT;
        }

        public override void SetVariable(object data)
        {
            runTimeData = data;
        }

        public override void SetVariable(bool data)
        {
            runTimeData = data;
        }
        public override void SetVariable(float data)
        {
            runTimeData = data;
        }
        public override void SetVariable(float2 data)
        {
            runTimeData = data;
        }
        public override void SetVariable(float3 data)
        {
            runTimeData = data;
        }
        public override void SetVariable(float4 data)
        {
            runTimeData = data;
        }
        public override void SetVariable(string data)
        {
            runTimeData = data;
        }
        public override void SetVariable(int data)
        {
            runTimeData = data;
        }
        public override string ToString()
        {
            return this.runTimeData.ToString();
        }
        public override object GetVariable()
        {
            return runTimeData;
        }

        public override bool GetBoolVariable()
        {
            return (bool)runTimeData;
        }

        public override float4 GetColorVariable()
        {
            return (float4) runTimeData;
        }

        public override float3 GetFloat3Variable()
        {
            return (float3) runTimeData;
        }

        public override float2 GetFloat2Variable()
        {
            return (float2) runTimeData;
        }

        public override float GetFloatVariable()
        {
            return (float) runTimeData;
        }

        public override int GetIntVariable()
        {
            return (int) runTimeData;
        }

        public override string GetStringVariable()
        {
            return runTimeData?.ToString();
        }

        public override void CopyTo(IVariable variable)
        {

        }
    }
}
