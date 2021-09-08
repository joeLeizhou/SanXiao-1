using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(4, "数学", "Vector3", "距离(dist)")]
    public class Vector3Distance : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3Distance()
        {

        }
#if UNITY_EDITOR
        public Vector3Distance(int id) : base(id)
        {
            Name = "Vector3距离";
            Inputs = new Slot[2] { new Slot(id, true, "点1", new VVector3()), new Slot(id, true, "点2", new VVector3()) };
            Outputs = new Slot[1] { new Slot(id, false, "距离", new VFloat()) };
        }
#endif
        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var output = math.sqrt(math.distancesq(Inputs[0].GetFloat3Variable(), Inputs[1].GetFloat3Variable()));
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}