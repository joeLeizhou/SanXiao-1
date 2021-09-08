using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("条件分支", "真假门")]
    public class TFGate : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public TFGate()
        {

        }
#if UNITY_EDITOR
        public TFGate(int id) : base(id)
        {
            Name = "分支";
            Inputs = new Slot[] {
                new Slot(id, true, "条件", new VBool()),
            };
            Outputs = new Slot[] {
                new Slot(id, false, "假", new VFlow()),
                new Slot(id, false, "真", new VFlow()),
            };
        }
#endif

        public override bool CanReuse()
        {
            return true;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var condition = Inputs[0].GetBoolVariable();
            if (condition)
            {
                for (int i = 0; i < Outputs[1].LinkedSlots.Count; i++)
                {
                    graph.GetSlotById(Outputs[1].LinkedSlots[i]).Enable(graph);
                }
            }
            else
            {
                for (int i = 0; i < Outputs[0].LinkedSlots.Count; i++)
                {
                    graph.GetSlotById(Outputs[0].LinkedSlots[i]).Enable(graph);
                }
            }
            return frameCount;
        }
    }
}
