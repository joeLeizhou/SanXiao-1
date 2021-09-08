#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif
using Unity.Mathematics;

namespace Orcas.Graph.Core
{
    public abstract class IVariable
    {
        public abstract VariableType GetDataType();
        public virtual void SetVariable(object data) { }
        public virtual void SetVariable(bool data) { }
        public virtual void SetVariable(float4 data) { }
        public virtual void SetVariable(float data) { }
        public virtual void SetVariable(int data) { }
        public virtual void SetVariable(string data) { }
        public virtual void SetVariable(float2 data) { }
        public virtual void SetVariable(float3 data) { }
        public virtual object GetVariable() { return null; }
        public virtual bool GetBoolVariable() { return false; }
        public virtual float4 GetColorVariable() { return float4.zero; }
        public virtual float GetFloatVariable() { return 0f; }
        public virtual int GetIntVariable() { return 0; }
        public virtual string GetStringVariable() { return string.Empty; }
        public virtual float2 GetFloat2Variable() { return float2.zero; }
        public virtual float3 GetFloat3Variable() { return float3.zero; }
        public abstract void ResetVariable();
        public abstract void CopyTo(IVariable variable);
        public abstract bool CheckCastDataType(VariableType dataType);
#if UNITY_EDITOR
        public virtual VisualElement InstantiateControl() { return null; }
#endif
    }
}
