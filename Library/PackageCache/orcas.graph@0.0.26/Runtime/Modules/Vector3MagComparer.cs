using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(8, "数学", "Vector3", "比较长度(compare)")]
    public class Vector3MagComparer : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3MagComparer()
        {

        }
#if UNITY_EDITOR
        public Vector3MagComparer(int id) : base(id)
        {
            Name = "Vector3比较长度";
            Inputs = new Slot[2] {
                new Slot(id, true, "向量", new VVector3()),
                new Slot(id, true, "长度", new VFloat())
            };
            Outputs = new Slot[1] { new Slot(id, false, "长度是否大于给定长度", new VBool()) };
        }
#endif
        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var a = Inputs[0].GetFloat3Variable();
            var b = Inputs[1].GetFloatVariable();

            var output = math.lengthsq(a) > math.lengthsq(b);

            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}