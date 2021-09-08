using Orcas.Graph.Core;
using Orcas.Graph.Variables;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("条件分支", "比较整数大小(compare_int)")]
    public class CompareInt : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        public CompareInt()
        {

        }
#if UNITY_EDITOR
        public CompareInt(int id) : base(id)
        {
            Name = "比较整数大小";
            Inputs = new Slot[] {
                new Slot(id, true, "x", new VInt()),
                new Slot(id, true, "y", new VInt())
            };
            Outputs = new Slot[]
            {
                new Slot(id, false, "x大于y", new VFlow()),
                new Slot(id, false, "x等于y", new VFlow()),
                new Slot(id, false, "x小于y", new VFlow())
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
            if (x > y)
            {
                OutputFlow(graph, 0);
            }

            else if (x == y)
            {
                OutputFlow(graph, 1);
            }
            else
            {
                OutputFlow(graph, 2);
            }

            return frameCount;
        }
    }
}