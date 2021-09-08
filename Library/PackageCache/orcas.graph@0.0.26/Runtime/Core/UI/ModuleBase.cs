using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Orcas.Graph.Core
{
    [UnityEngine.Scripting.Preserve]
    public abstract class ModuleBase : UIEntity
    {
        [UnityEngine.Scripting.Preserve]
        public string Name { get; set; }
        [UnityEngine.Scripting.Preserve]
        public Rect Rect { get; set; }
        [UnityEngine.Scripting.Preserve]
        public Slot[] Inputs { get; set; }
        [UnityEngine.Scripting.Preserve]
        public Slot[] Outputs { get; set; }

        [UnityEngine.Scripting.Preserve]
        public ModuleBase()
        {

        }

        protected abstract int Update(Graph graph, GraphContext context, int frameCount, bool isInit);

        public virtual bool CanReuse() { return false; } // 不等待输入，没有状态的模块

        public int UpdateInGraph(Graph graph, GraphContext context, int frameCount, bool isInit)
        {
            return Update(graph, context, frameCount, isInit);
        }

        protected void OutputFlow(Graph graph, int index)
        {
            for (var i = 0; i < Outputs[index].LinkedSlots.Count; i++)
            {
                graph.GetSlotById(Outputs[index].LinkedSlots[i]).Enable(graph);
            }
        }

#if UNITY_EDITOR
        public ModuleBase(int id)
        {
            this.ID = id;
            Rect = new Rect(10, 10, 100, 100);
            Inputs = new Slot[0];
            Outputs = new Slot[0];
        }

        public void GetSlots(List<Slot> list)
        {
            list.AddRange(Inputs);
            list.AddRange(Outputs);
        }

        public void CopyDatasTo(ModuleBase other)
        {
            if (this.GetType() != other.GetType())
                return;
            for (int i = 0; i < this.Inputs.Length; i++)
            {
                this.Inputs[i].Data.CopyTo(other.Inputs[i].Data);
            }
        }
#endif
    }
}
