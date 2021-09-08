using Newtonsoft.Json;
using System;
using System.Linq;
using Orcas.Graph.Core;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace Orcas.Graph.Variables
{
    [UnityEngine.Scripting.Preserve]
    public class VEnum : IVariable
    {
        [UnityEngine.Scripting.Preserve]
        public int data { get; set; }
        [UnityEngine.Scripting.Preserve]
        public string eTypeName { get; set; }
        [UnityEngine.Scripting.Preserve]
        private int runTimeData;
        [UnityEngine.Scripting.Preserve]
        public VEnum()
        {

        }
        [UnityEngine.Scripting.Preserve]
        public VEnum(Enum d)
        {
            data = (int)Convert.ChangeType(d, typeof(int));
            eTypeName = $"{d.GetType().FullName},{d.GetType().Assembly}";
        }
        public override bool CheckCastDataType(VariableType dataType)
        {
            return dataType == VariableType.INT || dataType == VariableType.ENUM || dataType == VariableType.OBJECT;
        }

        public override void ResetVariable()
        {
            runTimeData = data;
        }

        public override VariableType GetDataType()
        {
            return VariableType.ENUM;
        }

        public override void SetVariable(object data)
        {
            runTimeData = (int) data;
        }

        public override void SetVariable(int data)
        {
            runTimeData = data;
        }

        public override int GetIntVariable()
        {
            return runTimeData;
        }

        public override string ToString()
        {
            return this.data.ToString();
        }

        public override void CopyTo(IVariable variable)
        {
            ((VEnum)variable).data = this.data;
        }
#if UNITY_EDITOR
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement() { name = "inputContainer" };
            var dummy = new VisualElement { name = "dummy" };
            dummy.Add(new Label("Type"));
            container.Add(dummy);
            var eType = Type.GetType(eTypeName);
            var eValue = (Enum)Enum.ToObject(eType, data);
            VisualElement f = null;
            if (eType.IsDefined(typeof(FlagsAttribute), false) == false)
            {
                var field = new EnumField() { value = eValue };
                field.Init(eValue);
                field.RegisterValueChangedCallback(evt =>
                {
                    data = (int)Convert.ChangeType(evt.newValue, typeof(int));
                });
                f = field;
            }
            else
            {
                var field = new EnumFlagsField() { value = eValue };
                field.Init(eValue);
                field.RegisterValueChangedCallback(evt =>
                {
                    data = (int)Convert.ChangeType(evt.newValue, typeof(int));
                });
                f = field;
            }

            var text = f.Children().ToArray()[0].Children().OfType<TextElement>().FirstOrDefault();
            var eValueMemberInfo = eType.GetMember(eValue.ToString());
            if (eValueMemberInfo.Length > 0 && Attribute.IsDefined(eValueMemberInfo[0], typeof(InspectorNameAttribute)))
            {
                var inspectorName = (InspectorNameAttribute)(eValueMemberInfo[0].GetCustomAttributes(typeof(InspectorNameAttribute), false)[0]);
                text.text = inspectorName.displayName;
            }
            else
            {
                text.text = ObjectNames.NicifyVariableName(eValue.ToString());
            }

            container.Add(f);
            return container;
        }
#endif
    }
}
