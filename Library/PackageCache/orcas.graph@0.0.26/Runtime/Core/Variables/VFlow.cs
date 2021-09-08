using Orcas.Graph.Core;

namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VFlow : IVariable
    {
        public override bool CheckCastDataType(VariableType dataType)
        {
            return true;
        }

        public override VariableType GetDataType()
        {
            return VariableType.FLOW;
        }

        public override void CopyTo(IVariable variable)
        {

        }

        public override void ResetVariable()
        {

        }
    }
}
