using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("流程", "等待(wait)", "几帧(frame)")]
    public class WaitForFrame : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public WaitForFrame()
        {
        }
#if UNITY_EDITOR
        public WaitForFrame(int id) : base(id)
        {
            Name = "等待几帧";
            Inputs = new Slot[] {
                new Slot(id, true, "帧数", new VInt()),
                new Slot(id, true, "攻速", new VFloat(100f)),
                new Slot(id, true, "流", new VFlow())
            };
            Outputs = new Slot[1] { new Slot(id, false, "流", new VFlow()) };
        }
#endif

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            if (isInit)
            {
                var speed = Inputs[1].GetFloatVariable();
                if (speed > 100)
                {
                    speed = (speed - 100) / 2 + 100;
                }
                var waitTime = (int) math.round(Inputs[0].GetIntVariable() * 101f / (speed + 1f));
                if (waitTime > 0)
                {
                    return frameCount + waitTime;
                }
            }

            OutputFlow(graph, 0);
            return frameCount;
        }
    }
}
