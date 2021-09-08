using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(2, "数学", "Vector3", "获取单位向量(normalize)")]
    public class Vector3normalize : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3normalize()
        {

        }
#if UNITY_EDITOR
        public Vector3normalize(int id) : base(id)
        {
            Name = "Vector3获取单位向量";
            Inputs = new Slot[1] { new Slot(id, true, "向量", new VVector3()) };
            Outputs = new Slot[1] { new Slot(id, false, "单位向量", new VVector3()) };
        }
#endif
        public override bool CanReuse()
        {
            return true;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var output = math.normalize(Inputs[0].GetFloat3Variable());
            for (int i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}