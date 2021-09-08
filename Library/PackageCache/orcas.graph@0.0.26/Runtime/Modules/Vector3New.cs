using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(9, "数学", "Vector3", "构造(new)")]
    public class Vector3New : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3New()
        {

        }
#if UNITY_EDITOR
        public Vector3New(int id) : base(id)
        {
            Name = "构造Vector3";
            Inputs = new Slot[3] { new Slot(id, true, "x分量", new VFloat()), new Slot(id, true, "y分量", new VFloat()), new Slot(id, true, "z分量", new VFloat()) };
            Outputs = new Slot[1] { new Slot(id, false, "结果", new VVector3()) };
        }
#endif

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var x = Inputs[0].GetFloatVariable();
            var y = Inputs[1].GetFloatVariable();
            var z = Inputs[2].GetFloatVariable();

            var output = new float3(x, y, z);
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}