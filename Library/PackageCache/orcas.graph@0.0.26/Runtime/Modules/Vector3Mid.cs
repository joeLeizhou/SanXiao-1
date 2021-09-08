using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(5, "数学", "Vector3", "平均值(mid)")]
    public class Vector3Mid : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3Mid()
        {

        }
#if UNITY_EDITOR
        public Vector3Mid(int id) : base(id)
        {
            Name = "Vector3平均值";
            Inputs = new Slot[2] { new Slot(id, true, "向量1", new VVector3()), new Slot(id, true, "向量2", new VVector3()) };
            Outputs = new Slot[1] { new Slot(id, false, "平均值", new VVector3()) };
        }
#endif
        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var output = (Inputs[0].GetFloat3Variable() + Inputs[1].GetFloat3Variable()) / 2;
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}