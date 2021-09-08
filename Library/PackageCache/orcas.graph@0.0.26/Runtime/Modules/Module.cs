using Orcas.Graph.Variables;
using Orcas.Graph.Core;

namespace Orcas.Graph.Module
{
    [UnityEngine.Scripting.Preserve]
    [GraphModule("流程", "执行一个图")]
    public class Module : ModuleBase
    {
        [UnityEngine.Scripting.Preserve]
        private GraphContext initData1;
        [UnityEngine.Scripting.Preserve]
        private Core.Graph graph1;
        [UnityEngine.Scripting.Preserve]
        public Module()
        {

        }
#if UNITY_EDITOR
        public Module(int id) : base(id)
        {
            Name = "执行一个graph";
            Inputs = new[] {
                new Slot(id, true, "图名字", new VString())
            };
        }
#endif

        protected override int Update(Core.Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            if (isInit)
            {
                initData1 = context.Copy();
                initData1.GraphTempId = context.GraphTempId;
                initData1.Name = Inputs[0].GetStringVariable();
                graph1 = GraphPool.GetGraph(initData1.Name);
            }
            if (GraphManager.CheckRuntimeGraphInterrupt(initData1.GraphTempId))
            {
                GraphPool.CollectGraph(initData1.Name, graph1);
            }
            else if (graph1.UpdateRun(initData1))
            {
                return frameCount + 1;
            }
            else
            {
                GraphPool.CollectGraph(initData1.Name, graph1);
            }

            for (var i = 0; i < Outputs[0].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[0].LinkedSlots[i]).Enable(graph);
            }

            return frameCount;
        }
    }
}