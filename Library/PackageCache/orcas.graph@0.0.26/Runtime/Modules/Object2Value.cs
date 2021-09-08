using Orcas.Graph.Variables;
using Orcas.Graph.Core;
using Unity.Mathematics;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("数据", "object", "显示转换数据类型")]
    public class Object2Value : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public Object2Value()
        {

        }
#if UNITY_EDITOR
        public Object2Value(int id) : base(id)
        {
            Name = "显示转换数据类型";
            Inputs = new[] {
                new Slot(id, true, "对象", new VObject())
            };
            Outputs = new[] {
                new Slot(id, false, "整数", new VInt()),
                new Slot(id, false, "浮点数", new VFloat()),
                new Slot(id, false, "二维向量", new VVector2()),
                new Slot(id, false, "三维向量", new VVector3()),
                new Slot(id, false, "布尔", new VBool()),
                new Slot(id, false, "字符串", new VString())
            };
        }
#endif

        public override bool CanReuse()
        {
            return true;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var obj = Inputs[0].GetVariable() ;
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable((int)obj, graph);
            }
            for (var i = 0; i < Outputs[1].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[1].LinkedSlots[i]).SetVariableAndEnable((float)obj, graph);
            }
            for (var i = 0; i < Outputs[2].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[2].LinkedSlots[i]).SetVariableAndEnable((float2)obj, graph);
            }
            for (var i = 0; i < Outputs[3].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[3].LinkedSlots[i]).SetVariableAndEnable((float3)obj, graph);
            }
            for (var i = 0; i < Outputs[4].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[4].LinkedSlots[i]).SetVariableAndEnable((bool)obj, graph);
            }
            for (var i = 0; i < Outputs[5].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[5].LinkedSlots[i]).SetVariableAndEnable(obj.ToString(), graph);
            }
            return frameCount;
        }
    }
}