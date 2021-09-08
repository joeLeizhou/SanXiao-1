using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("数据", "object", "转换为字符串(toString)")]
    public class ToString : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public ToString()
        {

        }
#if UNITY_EDITOR
        public ToString(int id) : base(id)
        {
            Name = "ToString";
            Inputs = new [] {
                new Slot(id, true, "对象", new VObject()),
            };
            Outputs = new [] {
                new Slot(id, false, "输出", new VString())
            };
        }
#endif

        public override bool CanReuse()
        {
            return true;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var obj = Inputs[0].GetVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(obj.ToString(), graph);
            }
            return frameCount;
        }
    }
}