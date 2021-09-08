using System;
using System.Collections.Generic;
using System.IO;
using Orcas.Core.Tools;
using UnityEditor;
using UnityEngine;

namespace Orcas.Graph.Core
{
    public partial class Graph
    {
#if UNITY_EDITOR

        private string _path;

        [NonSerialized]
        public List<SerializedEdge> SerializedEdges;
        
        public void SetPath(string path)
        {
            _path = Path.GetDirectoryName(path) + '/' + Path.GetFileNameWithoutExtension(path);
        }

        public string GetPath()
        {
            return _path;
        }
        
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(_path) == true)
            {
                _path = "Assets/New Graph.asset";
            }
            Modules = new List<ModuleBase>();
            _entityDict = new Dictionary<int, UIEntity>();
            SerializedEdges = new List<SerializedEdge>();
            Top = 0;
        }
        public void RegisterSlot(Slot slot)
        {
            var tempId = Top++;
            slot.ID = tempId;
            _entityDict.Add(tempId, slot);
        }

        public ModuleBase CreateModule(Type moduleType)
        {
            var tempId = Top++;
            var module = moduleType.GetConstructor(new Type[] { typeof(int) })?.Invoke(new object[] { tempId }) as ModuleBase;
            _entityDict.Add(tempId, module);
            Modules.Add(module);
            for (var i = 0; i < module.Inputs.Length; i++)
                RegisterSlot(module.Inputs[i]);
            for (var i = 0; i < module.Outputs.Length; i++)
                RegisterSlot(module.Outputs[i]);
            return module;
        }
        public void RegisterCompleteObjectUndo(string name)
        {

        }

        public void Save()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var savePath = _path;
            ScriptableObject data = null;
            data = CreateInstance(typeof(Graph));
            (data as Graph)?.CopyFrom(this);
            if (string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.CreateAsset(data, savePath + ".asset");
            }
            else
            {
                savePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assetPath) + "/", System.IO.Path.GetFileNameWithoutExtension(assetPath));
                AssetDatabase.CreateAsset(data, assetPath);
            }
            var bytes = SerializeTools.GetObjectBytes(data as Graph);

            Debug.Log("SaveMapPath:" + savePath);
            var fs = File.Open(savePath + "b.bytes", FileMode.Create, FileAccess.Write);
            if (fs.CanWrite)
            {
                fs.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Close();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public void GetEdges(int moduleID, int slotID, List<SerializedEdge> list)
        {
            for (var i = 0; i < SerializedEdges.Count; i++)
            {
                var e = SerializedEdges[i];
                if (e.SourceNodeGuid == moduleID && e.SourceSlotGuid == slotID)
                    list.Add(e);
                else if (e.TargetNodeGuid == moduleID && e.TargetSlotGuid == slotID)
                    list.Add(e);
            }
        }
        public SerializedEdge FindEdge(int sourceSlotID, int targetSlotID)
        {
            for (var i = 0; i < SerializedEdges.Count; i++)
            {
                var e = SerializedEdges[i];
                if (e.SourceSlotGuid == sourceSlotID && e.TargetSlotGuid == targetSlotID
                 || e.TargetSlotGuid == sourceSlotID && e.SourceSlotGuid == targetSlotID)
                {
                    return e;
                }
            }
            return null;
        }
        public void RemoveModule(int id)
        {
            var module = GetModuleById(id);
            if (module == null) return;
            Modules.Remove(module);
            _entityDict.Remove(module.ID);
            for (var i = SerializedEdges.Count - 1; i >= 0; i--)
            {
                if (SerializedEdges[i].TargetNodeGuid == id || SerializedEdges[i].SourceNodeGuid == id)
                    SerializedEdges.RemoveAt(i);
            }
            for (var i = 0; i < module.Inputs.Length; i++)
            {
                _entityDict.Remove(module.Inputs[i].ID);
            }
            for (var i = 0; i < module.Outputs.Length; i++)
            {
                _entityDict.Remove(module.Outputs[i].ID);
            }
        }
        
        public bool AddEdge(Slot sourceSlot, Slot targetSlot, SerializedEdge edge)
        {
            if (sourceSlot.LinkedSlots.Contains(targetSlot.ID) || targetSlot.LinkedSlots.Contains(sourceSlot.ID))
                return false;
            SerializedEdges.Add(edge);
            sourceSlot.LinkedSlots.Add(targetSlot.ID);
            targetSlot.LinkedSlots.Add(sourceSlot.ID);
            return true;
        }

        public void RemoveEdge(SerializedEdge edge)
        {
            SerializedEdges.Remove(edge);
            GetSlotById(edge.SourceSlotGuid).LinkedSlots.Remove(edge.TargetSlotGuid);
            GetSlotById(edge.TargetSlotGuid).LinkedSlots.Remove(edge.SourceSlotGuid);
        }

        public void CopyFrom(Graph blackBoard)
        {
            Top = blackBoard.Top;
            Modules = new List<ModuleBase>(blackBoard.Modules);
            _entityDict = new Dictionary<int, UIEntity>();
            _path = blackBoard._path;
            SerializedEdges = new List<SerializedEdge>();
            var slots = new List<Slot>();
            //初始化查询表
            for (var i = 0; i < Modules.Count; i++)
            {
                _entityDict.Add(Modules[i].ID, Modules[i]);
                for (var j = 0; j < Modules[i].Inputs.Length; j++)
                {
                    _entityDict.Add(Modules[i].Inputs[j].ID, Modules[i].Inputs[j]);
                    slots.Add(Modules[i].Inputs[j]);
                }
                for (var j = 0; j < Modules[i].Outputs.Length; j++)
                {
                    _entityDict.Add(Modules[i].Outputs[j].ID, Modules[i].Outputs[j]);
                    slots.Add(Modules[i].Outputs[j]);
                }
            }
            //初始化slot关系图
            for (var i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsInput)
                    continue;

                for (var j = 0; j < slots[i].LinkedSlots.Count; j++)
                {
                    SerializedEdges.Add(new SerializedEdge()
                    {
                        SourceNodeGuid = slots[i].ModId,
                        SourceSlotGuid = slots[i].ID,
                        TargetNodeGuid = GetSlotById(slots[i].LinkedSlots[j]).ModId,
                        TargetSlotGuid = slots[i].LinkedSlots[j],
                    });
                }
            }
        }
#endif
    }
}