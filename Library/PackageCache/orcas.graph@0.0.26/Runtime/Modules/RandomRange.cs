using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Orcas.Core.Tools;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(0, "数学", "Float", "范围随机(random)")]
    public class RandomRange : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        private int statePamators = -1;
        [UnityEngine.Scripting.Preserve]
        public RandomRange()
        {
        }
#if UNITY_EDITOR
        public RandomRange(int id) : base(id)
        {
            Name = "在指定范围内随机";
            Inputs = new [] {
                new Slot(id, true, "最小值", new VFloat()),
                new Slot(id, true, "最大值", new VFloat())
            };
            Outputs = new [] {
                new Slot(id, false, "随机数", new VFloat()),
            };
        }
#endif
        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var a = Inputs[0].GetFloatVariable();
            var b = Inputs[1].GetFloatVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(Utils.RandRange(a, b), graph);
            }
            return frameCount;
        }
    }
}
