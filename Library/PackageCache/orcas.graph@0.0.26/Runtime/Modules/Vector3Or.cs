using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(0, "数学", "Vector3", "或(or)")]
    public class Vector3Or : OrModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3Or()
        {

        }
#if UNITY_EDITOR
        public Vector3Or(int id) : base(id)
        {
            Name = "Or";
            Inputs = new Slot[2] { new Slot(id, true, "x", new VVector3()), new Slot(id, true, "y", new VVector3()) };
            Outputs = new Slot[1] { new Slot(id, false, "output", new VVector3()) };
        }
#endif

        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var output = EnabledSlotID == Inputs[0].ID ? Inputs[0].GetFloat3Variable() : Inputs[1].GetFloat3Variable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}