using Orcas.Graph.Core;
using Orcas.Graph.Variables;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("数学", "int", "取模(mod)")]
    public class IntMod : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public IntMod() : base()
        {

        }
#if UNITY_EDITOR
        public IntMod(int id) : base(id)
        {
            Name = "整数取模";
            Inputs = new[] {
                new Slot(id, true, "x", new VInt()),
                new Slot(id, true, "y", new VInt())
            };
            Outputs = new[]
            {
                new Slot(id, false, "x%y", new VInt()),
            };
        }
#endif
        public override bool CanReuse()
        {
            return Inputs[0].LinkedSlots.Count + Inputs[1].LinkedSlots.Count <= 1;
        }

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            var x = Inputs[0].GetIntVariable();
            var y = Inputs[1].GetIntVariable();
            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(x % y, graph);
            }
    
            return frameCount;
        }
    }
}