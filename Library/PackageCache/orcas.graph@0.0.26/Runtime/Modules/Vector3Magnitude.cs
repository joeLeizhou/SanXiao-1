using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(3, "数学", "Vector3", "获取长度(len)")]
    public class Vector3Magnitude : ModuleBase
    {
        [Newtonsoft.Json.JsonConstructor]
        public Vector3Magnitude()
        {

        }
#if UNITY_EDITOR
        public Vector3Magnitude(int id) : base(id)
        {
            Name = "Vector3获取长度";
            Inputs = new Slot[1] {
                new Slot(id, true, "向量", new VVector3())
            };
            Outputs = new Slot[1] { new Slot(id, false, "长度", new VFloat()) };
        }
#endif
        public override bool CanReuse()
        {
            return true;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var a = Inputs[0].GetFloat3Variable();
            var output = math.length(a);

            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}