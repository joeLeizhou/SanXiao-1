using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(3, "数学", "Float", "缩放(scale)")]
    public class FloatScaling : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public FloatScaling() : base()
        {
        }
#if UNITY_EDITOR
        public FloatScaling(int id) : base(id)
        {
            Name = "Float 缩放";
            Inputs = new Slot[3] {
                new Slot(id, true, "数", new VFloat()),
                new Slot(id, true, "乘以", new VFloat(1f)),
                new Slot(id, true, "除以", new VFloat(1f))
            };
            Outputs = new Slot[1] {
                 new Slot(id, false, "结果", new VFloat())
            };
        }
#endif
        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count + Inputs[2].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var output = Inputs[0].GetFloatVariable() * Inputs[1].GetFloatVariable() / Inputs[2].GetFloatVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}