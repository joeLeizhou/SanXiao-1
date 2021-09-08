using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(10, "数学", "Vector3", "分离(split)")]
    public class Vector3Split : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3Split()
        {

        }
#if UNITY_EDITOR
        public Vector3Split(int id) : base(id)
        {
            Name = "Vector3分离";
            Inputs = new Slot[1] { new Slot(id, true, "Vector3", new VVector3()) };
            Outputs = new Slot[3]
            {
                new Slot(id, false, "x", new VFloat()),
                new Slot(id, false, "y", new VFloat()),
                new Slot(id, false, "z", new VFloat())
            };
        }
#endif
        public override bool CanReuse()
        {
            return true;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var v3 = Inputs[0].GetFloat3Variable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(v3.x, graph);
            }
            for (var i = 0; i < Outputs[1].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[1].LinkedSlots[i]).SetVariableAndEnable(v3.y, graph);
            }
            for (var i = 0; i < Outputs[2].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[2].LinkedSlots[i]).SetVariableAndEnable(v3.z, graph);
            }
            return frameCount;
        }
    }
}