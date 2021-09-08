using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Orcas.Core.Tools;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule(6, "数学", "Vector3", "单范围随机(random)")]
    public class RandomVector3 : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        private int _statePamators = -1;
        [UnityEngine.Scripting.Preserve]
        public RandomVector3()
        {
        }
#if UNITY_EDITOR
        public RandomVector3(int id) : base(id)
        {
            Name = "Vector3范围随机";
            Inputs = new [] {
                new Slot(id, true, "最小值", new VFloat()),
                new Slot(id, true, "最大值", new VFloat())
            };
            Outputs = new [] {
                new Slot(id, false, "随机数", new VVector3()),
            };
        }
#endif

        public override bool CanReuse()
        {
            return (Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count) <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var a = Inputs[0].GetFloatVariable();
            var b = Inputs[1].GetFloatVariable();
            var ret = float3.zero;
            ret.x = Utils.RandRange(a, b);
            ret.z = Utils.RandRange(a, b);
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(ret, graph);
            }
            return frameCount;
        }
    }
}
