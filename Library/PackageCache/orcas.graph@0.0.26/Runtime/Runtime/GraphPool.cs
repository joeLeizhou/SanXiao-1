using System.Collections.Generic;
using System;
using Orcas.Graph.Core;
using UnityEngine;

namespace Orcas.Graph.Core
{

    public class GraphPool
    {
        private readonly Queue<Graph> _queue;
        private Graph _graphOri;
        internal GraphPool(string path)
        {
            _graphOri = Graph.Load(path);
            _queue = new Queue<Graph>();
        }
        private readonly Type _slotType = typeof(Slot);
        internal Graph GetGraph()
        {
            var graph = _queue.Count > 0 ? _queue.Dequeue() : ScriptableObject.CreateInstance(typeof(Graph)) as Graph;
            if (graph.Modules != null && graph.Modules.Count > 0)
                return graph;
            graph.Modules = new List<ModuleBase>();
            var modulesOri = _graphOri.Modules;
            for (var i = 0; i < modulesOri.Count; i++)
            {
                if (modulesOri[i].CanReuse())
                {
                    graph.Modules.Add(modulesOri[i]);
                    continue;
                }
                var module = (ModuleBase)Activator.CreateInstance(modulesOri[i].GetType());
                var inputs = module.Inputs;
                if (inputs == null || inputs.Length != modulesOri[i].Inputs.Length)
                    inputs = new Slot[modulesOri[i].Inputs.Length];
                var outputs = module.Outputs;
                if (outputs == null || outputs.Length != modulesOri[i].Outputs.Length)
                    outputs = new Slot[modulesOri[i].Outputs.Length];
                for (var j = 0; j < modulesOri[i].Inputs.Length; j++)
                {
                    if (modulesOri[i].Inputs[j].LinkedSlots.Count == 0)
                    {
                        inputs[j] = modulesOri[i].Inputs[j];
                        continue;
                    }
                    inputs[j] = (Slot)Activator.CreateInstance(_slotType);
                    inputs[j].Data = (IVariable)Activator.CreateInstance(modulesOri[i].Inputs[j].Data.GetType());
                    modulesOri[i].Inputs[j].CopyTo(inputs[j]);
                }
                for (var j = 0; j < modulesOri[i].Outputs.Length; j++)
                {
                    if (modulesOri[i].Outputs[j].LinkedSlots.Count == 0)
                    {
                        outputs[j] = modulesOri[i].Outputs[j];
                        continue;
                    }
                    outputs[j] = (Slot)Activator.CreateInstance(_slotType);
                    outputs[j].Data = (IVariable)Activator.CreateInstance(modulesOri[i].Outputs[j].Data.GetType());
                    modulesOri[i].Outputs[j].CopyTo(outputs[j]);
                }
                module.ID = modulesOri[i].ID;
#if UNITY_EDITOR
                module.Name = modulesOri[i].Name;
                module.Rect = modulesOri[i].Rect;
#endif
                module.Inputs = inputs;
                module.Outputs = outputs;
                graph.Modules.Add(module);
            }
            return graph;
        }

        internal void CollectGraph(Graph graph)
        {
            _queue.Enqueue(graph);
        }

        internal void Clear()
        {
            _graphOri = null;
            _queue.Clear();
        }

        private static Dictionary<string, GraphPool> pools = new Dictionary<string, GraphPool>();
        public static Graph GetGraph(string path)
        {
            if (pools.ContainsKey(path) == false)
            {
                pools.Add(path, new GraphPool(path));
            }
            return pools[path].GetGraph();
        }
        internal static bool Exist(string path)
        {
            return pools.ContainsKey(path);
        }

        public static void CollectGraph(string path, Graph graph)
        {
            if (pools.ContainsKey(path))
                pools[path].CollectGraph(graph);
        }

        internal static void ClearAll()
        {
            foreach (var item in pools)
            {
                item.Value.Clear();
            }
            pools.Clear();
        }
    }
}