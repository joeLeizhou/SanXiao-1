using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("条件分支", "Float输出")]
    public class TFGateFloat : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public TFGateFloat()
        {

        }
#if UNITY_EDITOR
        public TFGateFloat(int id) : base(id)
        {
            Name = "条件Float输出";
            Inputs = new [] {
                new Slot(id, true, "条件", new VBool()),
                new Slot(id, true, "真", new VFloat()),
                new Slot(id, true, "假", new VFloat()),
            };
            Outputs = new [] {
                new Slot(id, false, "结果", new VFloat()),
            };
        }
#endif

        public override bool CanReuse()
        {
            return Inputs[1].LinkedSlots.Count + Inputs[2].LinkedSlots.Count == 0;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var condition = Inputs[0].GetBoolVariable();
            var result = condition ? Inputs[1].GetFloatVariable() : Inputs[2].GetFloatVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(result, graph);
            }
            return frameCount;
        }
    }
}
