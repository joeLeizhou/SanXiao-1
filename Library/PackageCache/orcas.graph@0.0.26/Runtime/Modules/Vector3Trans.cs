using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(10, "数学", "Vector3", "旋转(rotate)")]
    public class Vector3Trans : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Vector3Trans()
        {

        }
#if UNITY_EDITOR
        public Vector3Trans(int id) : base(id)
        {
            Name = "Vector3旋转";
            Inputs = new Slot[2] { new Slot(id, true, "向量x", new VVector3()), new Slot(id, true, "旋转角度", new VFloat()) };
            Outputs = new Slot[1] { new Slot(id, false, "结果", new VVector3()) };
        }
#endif

        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var direction = Inputs[0].GetFloat3Variable();
            var thetabais = Inputs[1].GetFloatVariable() / 180 * math.PI;
            var output = new float3(direction.x * math.cos(thetabais) - direction.z * math.sin(thetabais), 0, direction.z * math.cos(thetabais) + direction.x * math.sin(thetabais));
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}