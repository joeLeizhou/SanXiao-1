using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("条件分支", "或门(or)")]
    public class OrGate : OrModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public OrGate()
        {

        }
#if UNITY_EDITOR
        public OrGate(int id) : base(id)
        {
            Name = "或门";
            Inputs = new [] {
                new Slot(id, true, "流1", new VFlow()),
                new Slot(id, true, "流2", new VFlow()),
            };
            Outputs = new [] {
                new Slot(id, false, "流", new VFlow()),
            };
        }
#endif

        public override bool CanReuse()
        {
            return true;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).Enable(graph);
            }
            return frameCount;
        }
    }
}
