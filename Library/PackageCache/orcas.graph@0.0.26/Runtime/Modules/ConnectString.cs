using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("数据", "string", "连接字符串(connect_string)")]
    public class ConnectString : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public ConnectString()
        {

        }
#if UNITY_EDITOR
        public ConnectString(int id) : base(id)
        {
            Name = "链接字符串";
            Inputs = new[] {
                new Slot(id, true, "串1", new VString()),
                new Slot(id, true, "串2", new VString())
            };
            Outputs = new[] {
                new Slot(id, false, "输出", new VString())
            };
        }
#endif

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var str = Inputs[0].GetStringVariable() + Inputs[1].GetStringVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(str, graph);
            }
            return frameCount;
        }
    }
}