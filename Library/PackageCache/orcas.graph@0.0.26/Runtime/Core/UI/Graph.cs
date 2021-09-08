using System;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Core.Tools;

namespace Orcas.Graph.Core
{
    [UnityEngine.Scripting.Preserve]
    // #if UNITY_EDITOR
    public partial class Graph : ScriptableObject, IGraph
    // #else 
    // public partial class Graph : Object, IGraph 
    // #endif
    {
        public List<ModuleBase> Modules { get; set; }
        public int Top { get; set; }

        public static IGraphFileLoader FileLoader = new DefaultGraphFileLoader();
        private Dictionary<int, UIEntity> _entityDict;
        private List<ModuleBase> _topology;
        private int[] _inDegrees;
        private int[] _nextUpdateTimes;
        private int _finishedIndex;
        public bool ForceUpdate;
        private int _frameCount;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="world"></param>
        public void Init()
        {
            if (_topology != null && _entityDict != null)
                return;
            _entityDict = new Dictionary<int, UIEntity>();
            _inDegrees = new int[Modules.Count];
            _nextUpdateTimes = new int[Modules.Count];
            //初始化查询表
            for (var i = 0; i < Modules.Count; i++)
            {
                _entityDict.Add(Modules[i].ID, Modules[i]);
                _inDegrees[i] = 0;
                for (var j = 0; j < Modules[i].Inputs.Length; j++)
                {
                    _entityDict.Add(Modules[i].Inputs[j].ID, Modules[i].Inputs[j]);
                    if (Modules[i].Inputs[j].LinkedSlots.Count > 0)
                    {
                        _inDegrees[i]++;
                    }
                }
                for (var j = 0; j < Modules[i].Outputs.Length; j++)
                {
                    _entityDict.Add(Modules[i].Outputs[j].ID, Modules[i].Outputs[j]);
                }
            }
            _topology = new List<ModuleBase>(Modules.Count);
            for (var loop = 0; loop < Modules.Count; loop++)
            {
                var findHeader = false;
                for (var i = 0; i < Modules.Count; i++)
                {
                    if (_inDegrees[i] != 0) continue;
                    findHeader = true;
                    _topology.Add(Modules[i]);
                    for (var j = 0; j < Modules[i].Outputs.Length; j++)
                    {
                        for (var k = 0; k < Modules[i].Outputs[j].LinkedSlots.Count; k++)
                        {
                            var linkedModId = GetSlotById(Modules[i].Outputs[j].LinkedSlots[k]).ModId;
                            _inDegrees[FindModuleIndex(Modules, linkedModId)]--;
                        }
                    }
                    _inDegrees[i] = -1;
                    break;
                }

                if (findHeader) continue;
                Debug.LogError("模块可能链接成环了！");
                break;
            }
        }
        
        /// <summary>
        /// 重设参数
        /// </summary>
        public void ResetVariable()
        {
            for (var i = 0; i < _topology.Count; i++)
            {
                _inDegrees[i] = 0;
                for (var j = 0; j < _topology[i].Inputs.Length; j++)
                {
                    _topology[i].Inputs[j].ResetVariable();
                    if (_topology[i].Inputs[j].LinkedSlots.Count > 0)
                        _inDegrees[i]++;
                }
                _nextUpdateTimes[i] = -1;
            }
            _finishedIndex = 0;
            ForceUpdate = false;
            _frameCount = 0;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="context"></param>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public bool UpdateRun(GraphContext context)
        {
            if (GraphManager.Instance.EnableUpdate == false && ForceUpdate == false) return true;
            _frameCount++;
            var allNextUpdateTime = _frameCount;
            for (var i = _finishedIndex; i < _topology.Count; i++)
            {
                if (_nextUpdateTimes[i] < 0 && _inDegrees[i] > 0) // 模块未被激活
                    continue;
                if (_nextUpdateTimes[i] > 0 && _nextUpdateTimes[i] < _frameCount) // 模块已完成
                    continue;

                if (_nextUpdateTimes[i] < 0 || _nextUpdateTimes[i] == _frameCount) // 模块被激活或者到了下次更新的时间
                    _nextUpdateTimes[i] =
                        _topology[i].UpdateInGraph(this, context, _frameCount, _nextUpdateTimes[i] < 0);

                if (_nextUpdateTimes[i] > allNextUpdateTime)
                    allNextUpdateTime = _nextUpdateTimes[i];
                else if (_nextUpdateTimes[i] == _frameCount)
                    if (i == _finishedIndex)
                        _finishedIndex = i + 1;
            }

            return allNextUpdateTime > _frameCount;
        }

        /// <summary>
        /// 查询模块下标
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int FindModuleIndex(IReadOnlyList<ModuleBase> list, int id)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ID == id)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 激活模块
        /// </summary>
        /// <param name="id"></param>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public bool EnableModuleAndCheckReuse(int id, int slotId)
        {
            var index = FindModuleIndex(_topology, id);
            var module = _topology[index];
            if (module is OrModuleBase)
            {
                _inDegrees[index] = 0;
                (module as OrModuleBase).EnableSlotByID(slotId);
            }
            else
            {
                _inDegrees[index]--;
            }
            return _topology[index].CanReuse();
        }

        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ModuleBase GetModuleById(int id)
        {
            if (_entityDict.ContainsKey(id) == false) return null;
            return _entityDict[id] as ModuleBase;
        }
        
        /// <summary>
        /// 获取插槽
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Slot GetSlotById(int id)
        {
            if (_entityDict.ContainsKey(id) == false) return null;
            return _entityDict[id] as Slot;
        }
        
        /// <summary>
        /// 加载Graph
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Graph Load(string path)
        {
            var bytes = FileLoader.LoadGraphFile(path);
            var bf = new ByteBuffer(bytes.Length);
            bf.WriteBytes(bytes, 2);
            var len = bf.ReadUShort();
            bf.WriteBytes(bytes, len + 2);
            bf.ReadUShort();
            return SerializeTools.GetObject(typeof(Graph), bf) as Graph;
        }
    }

    [Serializable]
    public class SerializedNode
    {
        public int NodeGuid;
        public string NodeType;
        public string Json;
    }

    [Serializable]
    public class SerializedEdge : IEquatable<SerializedEdge>
    {
        public int SourceNodeGuid;
        public int SourceSlotGuid;
        public int TargetNodeGuid;
        public int TargetSlotGuid;


        public bool Equals(SerializedEdge other)
        {
            return this.SourceNodeGuid == other.SourceNodeGuid && this.SourceSlotGuid == other.SourceSlotGuid &&
             this.TargetNodeGuid == other.TargetNodeGuid && this.TargetSlotGuid == other.TargetSlotGuid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            return ReferenceEquals(this, obj) || this.Equals(obj as SerializedEdge);
        }

        public override int GetHashCode()
        {
            return (SourceNodeGuid << 12 | SourceSlotGuid) ^ (TargetNodeGuid << 12 | TargetSlotGuid);
        }

        public override string ToString()
        {
            return $"SourceNodeGuid:{SourceNodeGuid},SourceSlotGuid:{SourceSlotGuid},TargetNodeGuid:{TargetNodeGuid},TargetSlotGuid:{TargetSlotGuid}";
        }
    }
}
