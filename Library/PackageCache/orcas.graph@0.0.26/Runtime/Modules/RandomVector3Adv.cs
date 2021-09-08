using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Orcas.Core.Tools;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(7, "数学", "Vector3", "多范围随机(random)")]
    public class RandomVector3Adv : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        private int statePamators = -1;
        [UnityEngine.Scripting.Preserve]
        public RandomVector3Adv()
        {
        }
#if UNITY_EDITOR
        public RandomVector3Adv(int id) : base(id)
        {
            Name = "Vector3多范围随机";
            Inputs = new [] {
                new Slot(id, true, "x最小值", new VFloat()),
                new Slot(id, true, "x最大值", new VFloat()),
                new Slot(id, true, "z最小值", new VFloat()),
                new Slot(id, true, "z最大值", new VFloat())
            };
            Outputs = new [] {
                new Slot(id, false, "随机数", new VVector3()),
            };
        }
#endif

        public override bool CanReuse()
        {
            int linkCount = 0;
            for (int i = 0; i < Inputs.Length; i++)
                linkCount += Inputs[i].LinkedSlots.Count;
            return linkCount <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var xMin = Inputs[0].GetFloatVariable();
            var xMax = Inputs[1].GetFloatVariable();
            var zMin = Inputs[2].GetFloatVariable();
            var zMax = Inputs[3].GetFloatVariable();
            var ret = float3.zero;
            ret.x = Utils.RandRange(xMin, xMax);
            ret.z = Utils.RandRange(zMin, zMax);
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(ret, graph);
            }
            return frameCount;
        }
    }
}
