using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    public class LessthanOrEqualto : ModuleBase
    {
        [Newtonsoft.Json.JsonConstructor]
        public LessthanOrEqualto() : base()
        {

        }
#if UNITY_EDITOR
        public LessthanOrEqualto(int id) : base(id)
        {
            Name = "LessthanOrEqualto";
            Inputs = new Slot[3] { new Slot(id, true, "x", new VFloat()),
                                   new Slot(id, true, "y", new VFloat()),
                                   new Slot(id, true, "流", new VFlow()) };
            Outputs = new Slot[1] { new Slot(id, false, "output", new VBool()) };
        }
#endif

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            bool output = Inputs[0].GetFloatVariable() <= Inputs[1].GetFloatVariable();
            for (int i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).SetVariableAndEnable(output, graph);
            }
            return frameCount;
        }
    }
}
