using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using UnityEngine;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("流程", "等待(wait)", "几秒(sec),无视暂停")]
    public class WaitForRealSeconds : ModuleBase
    {
        private float startWaitTime;
        [UnityEngine.Scripting.Preserve]
        public WaitForRealSeconds()
        {
        }
#if UNITY_EDITOR
        public WaitForRealSeconds(int id) : base(id)
        {
            Name = "等待几秒，并且无视暂停";
            Inputs = new[] {
                new Slot(id, true, "秒数", new VFloat()),
                new Slot(id, true, "攻速", new VFloat(100f)),
                new Slot(id, true, "是否等待", new VBool(true)),
                new Slot(id, true, "流", new VFlow())
            };
            Outputs = new Slot[1] { new Slot(id, false, "流", new VFlow()) };
        }
#endif

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            if (isInit)
            {
                var isEnable = Inputs[2].GetBoolVariable();
                if (isEnable)
                {
                    var speed = Inputs[1].GetFloatVariable();
                    if (speed > 100)
                    {
                        speed = (speed - 100) / 2 + 100;
                    }
                    var waitTime = Inputs[0].GetFloatVariable() * 101f / (speed + 1f);
                    startWaitTime = Time.realtimeSinceStartup + waitTime;
                }
                else
                {
                    startWaitTime = 0f;
                }
            }

            if (Time.realtimeSinceStartup < startWaitTime)
            {
                return frameCount + 1;
            }

            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).Enable(graph);
            }
            return frameCount;
        }
    }
}
