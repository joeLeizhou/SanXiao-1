using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(11, "数学", "Vector3", "缩放(mul)")]
    public class Vector3Mul : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3Mul()
        {

        }
#if UNITY_EDITOR
        public Vector3Mul(int id) : base(id)
        {
            Name = "Vector3逐分量乘";
            Inputs = new Slot[2] { new Slot(id, true, "向量1", new VVector3()), new Slot(id, true, "缩放值", new VFloat(1f)) };
            Outputs = new Slot[1] { new Slot(id, false, "结果", new VVector3()) };
        }
#endif
        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var output = Inputs[0].GetFloat3Variable() * Inputs[1].GetFloatVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}