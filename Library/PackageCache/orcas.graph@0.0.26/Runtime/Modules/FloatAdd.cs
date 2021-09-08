using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(1, "数学", "Float", "加法(add)")]
    public class FloatAdd : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public FloatAdd() : base()
        {
        }
#if UNITY_EDITOR
        public FloatAdd(int id) : base(id)
        {
            Name = "Float加法";
            Inputs = new Slot[2] { new Slot(id, true, "数1", new VFloat()), new Slot(id, true, "数2", new VFloat()) };
            Outputs = new Slot[1] { new Slot(id, false, "结果", new VFloat()) };
        }
#endif

        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var output = Inputs[0].GetFloatVariable() + Inputs[1].GetFloatVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}