using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Orcas.Graph.Core
{
    [UnityEngine.Scripting.Preserve]
    public class Slot : UIEntity
    {
        [UnityEngine.Scripting.Preserve]
        public int ModId { get; set; }
        [UnityEngine.Scripting.Preserve]
        public string Name { get; set; } = "";
        [UnityEngine.Scripting.Preserve]
        public IVariable Data { get; set; }
        [UnityEngine.Scripting.Preserve]
        public List<int> LinkedSlots { get; set; }
        [UnityEngine.Scripting.Preserve]
        public bool IsInput { get; set; }
        [UnityEngine.Scripting.Preserve]
        public Slot()
        {
        }
        public VariableType GetDataType()
        {
            return Data.GetDataType();
        }
        public void CopyTo(Slot to)
        {
            to.ID = this.ID;
            to.ModId = this.ModId;
#if UNITY_EDITOR
            to.Name = this.Name;
#endif
            this.Data.CopyTo(to.Data);
            to.LinkedSlots = this.LinkedSlots;
            to.IsInput = this.IsInput;
        }
        public void SetVariableAndEnable(UnityEngine.Object data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable((object)data);
        }
        public void SetVariableAndEnable(object data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }
        public void SetVariableAndEnable(bool data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }
        public void SetVariableAndEnable(float4 data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }
        public void SetVariableAndEnable(float data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }
        public void SetVariableAndEnable(int data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }
        public void SetVariableAndEnable(string data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }
        public void SetVariableAndEnable(float2 data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }
        public void SetVariableAndEnable(float3 data, Graph graph)
        {
            Enable(graph);
            Data.SetVariable(data);
        }

        public void Enable(Graph graph)
        {
            graph.EnableModuleAndCheckReuse(ModId, ID);
        }
        public void ResetVariable()
        {
            Data.ResetVariable();
        }

        public object GetVariable()
        {
            return Data.GetVariable();
        }
        public bool GetBoolVariable()
        {
            return Data.GetBoolVariable();
        }
        public float4 GetColorVariable()
        {
            return Data.GetColorVariable();
        }
        public float GetFloatVariable()
        {
            return Data.GetFloatVariable();
        }
        public int GetIntVariable()
        {
            return Data.GetIntVariable();
        }
        public string GetStringVariable()
        {
            return Data.GetStringVariable();
        }
        public float2 GetFloat2Variable()
        {
            return Data.GetFloat2Variable();
        }
        public float3 GetFloat3Variable()
        {
            return Data.GetFloat3Variable();
        }
        public T GetEnumData<T>() where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), Data.GetIntVariable());
        }

#if UNITY_EDITOR
        public Slot(int module, bool isInput, string name, IVariable data)
        {
            ModId = module;
            this.IsInput = isInput;
            this.Name = name;
            this.Data = data;
            LinkedSlots = new List<int>();
        }

        public bool IsCompatibleWith(Slot other)
        {
            return Data.CheckCastDataType(other.Data.GetDataType());
        }

        public UnityEngine.UIElements.VisualElement InstantiateControl()
        {
            return Data.InstantiateControl();
        }

        public Color GetColor()
        {
            int index = (int)Data.GetDataType();
            var offset = index <= 9 ? 0.02f : 0.12f;
            return Color.HSVToRGB((index % 12) * 0.075f + offset, 0.725f, 0.9f);
        }
#endif
    }
}
