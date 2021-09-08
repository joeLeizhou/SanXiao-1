using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Orcas.Networking
{
    [Serializable]
    public class APIInfo
    {
        [Serializable]
        public enum Method
        {
            [InspectorName("Post(Form)")]
            POST_FORM,
            
            [InspectorName("Post(Url)")]
            POST_URL,

            [InspectorName("Post(WWWForm)")]
            POST_WWWForm,

            [InspectorName("Get")]
            GET,
        }

      
        [Serializable]
        public enum ValueType { 
            Sbyte,
            Byte,
            Short,
            Ushort,
            Int,
            Uint,
            Long,
            Ulong,
            Char,
            Float,
            Double,
            Bool,
            Decimal,
            String,
            Object,
            Dynamic
        }

        [Serializable]
        public enum Format {
            [InspectorName("C#")]
            CSharp,
            [InspectorName("Lua")]
            Lua,
            [InspectorName("C# and Lua")]
            Both
        }

        [Serializable]
        public struct ParameterInfo
        {
            
           
            public ValueType type;
            public string name;

            public ParameterInfo(ValueType type, string name)
            {
                this.type = type;
                this.name = name;
            }
        }

        [SerializeField]
        private string _path = "";

        [SerializeField]
        private Method _method;

        [SerializeField]
        private Format _format;

        [SerializeField]
        private List<ParameterInfo> _parameterList;

        [HideInInspector]
        public bool _parameterListVisible = true;

        [HideInInspector]
        public bool _visible = true;

        public string GetPath()
        {
            return _path;
        }

        public Method GetMethod()
        {
            return _method;
        }

        public Format GetFormat()
        {
            return _format;
        }

        public List<ParameterInfo> GetParameterList()
        {
            return _parameterList;
        }

        public APIInfo()
        {
            _path = "";
            _method = Method.POST_FORM;
            _parameterList = new List<ParameterInfo>();
        }

        public void AddNewParameter()
        {
            _parameterList.Add(new ParameterInfo(ValueType.Int, ""));
        }
    }
}