#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEngine.UI;
using System.Xml.Schema;
using System.Runtime.InteropServices;
using Orcas.Networking.Tcp;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.TextCore;

namespace Orcas.Networking
{
    [Serializable]
    public class ProtoConfig : ScriptableObject
    {

        [Serializable]
        public enum ProtoType { 
            [SerializeField,InspectorName("IReqProto")]
            Req,
            
            [SerializeField,InspectorName("IRltProto")]
            Rlt
        }



        [Serializable]
        public enum Language
        {
            [SerializeField, InspectorName("C#")]
            CSharp,
            [SerializeField, InspectorName("Lua")]
            Lua,
        }

        [Serializable]
        public struct ParaInfo
        {
      
            [SerializeField]
            public string ParaType;
            [SerializeField]
            public string ParaName;

            public ParaInfo(string paraName, string paraType)
            {
                ParaName = paraName;
                ParaType = paraType;
            }
        }

        [SerializeField]
        private string _protoName = "NewProto";

        [SerializeField]
        private ProtoType _protoType;

        [SerializeField]
        private Language _language;

        [SerializeField, HideInInspector]
        public int _helperIndex { get; private set; } = -1;

        [SerializeField]
        private List<ParaInfo> _paraList;

        [SerializeField, HideInInspector]
        private string _oldType = "";

        [SerializeField, HideInInspector]
        public string EditingString = "";

       // [HideInInspector]
       // public static List<Type> CSTypes { get; private set; }

        [HideInInspector]
        public static List<Type> BaseTypes { get; private set; }

        private List<string> _displayTypes;
        private bool _hasPerfectMatch = false;
        private List<string> _hiddenTypes;
        
        [HideInInspector]
        public List<string> DisplayTypes
        {
            get
            {
                return _displayTypes;
            }
            private set
            {

            }
        }

        [HideInInspector]
        public bool ParaListVisible = true;

        [HideInInspector]
        public static List<string> TotalTypes;

        public ProtoConfig()
        {
            if (_paraList == null)
                _paraList = new List<ParaInfo>();

            _displayTypes = new List<string>();
            _hiddenTypes = new List<string>();
            _helperIndex = -1;
        }

        private void Awake()
        {
            if (_paraList == null)
                _paraList = new List<ParaInfo>();

            _displayTypes = new List<string>();
            _hiddenTypes = new List<string>();
            _helperIndex = -1;
        }

        public void SetNewHelperIndex(int newIndex)
        {
            _hasPerfectMatch = false;
            _displayTypes.Clear();
            _displayTypes.AddRange(TotalTypes);
            _hiddenTypes.Clear();
            _helperIndex = newIndex;
            if (_helperIndex >= 0)
                AddChars();
        }


        [ExecuteInEditMode, InitializeOnLoadMethod]
        static void Init()
        {
            
           

            BaseTypes = new List<Type>();  
           // BaseTypes.Add(typeof(int));
            BaseTypes.Add(typeof(Int16));
            BaseTypes.Add(typeof(Int32));
            BaseTypes.Add(typeof(Int64));
            BaseTypes.Add(typeof(float));
            BaseTypes.Add(typeof(double));
            //BaseTypes.Add(typeof(uint));
            BaseTypes.Add(typeof(UInt16));
            BaseTypes.Add(typeof(UInt32));
            BaseTypes.Add(typeof(UInt64));
            BaseTypes.Add(typeof(sbyte));
           // BaseTypes.Add(typeof(short));
           // BaseTypes.Add(typeof(ushort));
            //BaseTypes.Add(typeof(ulong));
            BaseTypes.Add(typeof(byte)); ;
            BaseTypes.Add(typeof(decimal));
            BaseTypes.Add(typeof(bool));
            BaseTypes.Add(typeof(char));
            BaseTypes.Add(typeof(string));
            BaseTypes.Add(typeof(object));

          

            TotalTypes = new List<string>();
         
            
                        foreach (var type in BaseTypes)
                        {

                            TotalTypes.Add(type.Name.ToLower());
                           // Debug.Log("typeName: " + type.Name.ToLower());
                            //TotoalTypes.Add(type.FullName);
                        }

            TotalTypes.Add("int");

            TotalTypes.Add("float");

            TotalTypes.Add("uint");

            TotalTypes.Add("short");
            TotalTypes.Add("ushort");
            TotalTypes.Add("ulong");

          
            //  TotalTypes.Add("int");
            //  TotalTypes.Add("uint");

            
        }

        private void OnValidate()
        {
            AdjustProtoName();

            SetEditingValue();

            _hasPerfectMatch = false;
            // Debug.Log("Validate! _helperIndex = " + _helperIndex);
            Debug.Log("Editing str value: " + EditingString);
            // if (_helperIndex < 0) return;

            /*   Debug.Log("value: " + ParaList[_helperIndex].ParaType);

               for (int i = 0; i < ParaList.Count; i++)
               {
                   Debug.Log("list: " + ParaList[i].ParaName + " value: " + ParaList[i].ParaType);
               }*/

            if (_helperIndex >= 0)
            {
                if (EditingString != _oldType)
                {
                   // Debug.Log("setOldType");
                    if (_oldType.Length < EditingString.Length)
                    {
                      //  Debug.Log("Adding");
                        //增加字符
                        AddChars();
                    }
                    else
                    {
                       // Debug.Log("Deleting");
                        //删除字符
                        DelChars();
                    }

                    _oldType = EditingString;
                }
                else
                {
                    //  Debug.Log("heyheyhey!!");
                    //    _helperIndex = -1;
                }
            }
        }

        public void AddParaItem()
        {
            _paraList.Add(new ParaInfo("", ""));
        }

        public void DeleteParaItem(int index)
        {
            _paraList.RemoveAt(index);
        }

        private void AddChars()
        {
            var currentTypeName = EditingString.ToLower();
           // Debug.Log("currentTypeName: " + currentTypeName);
            for (int i = _displayTypes.Count - 1; i >= 0; i--)
            {
                var typeName = _displayTypes[i];
                
                if (string.Compare(currentTypeName, 0, typeName.ToLower(), 0, currentTypeName.Length) == 0)
                {
                    if (typeName.Length == currentTypeName.Length)
                    {
                        _displayTypes.RemoveAt(i);
                        _displayTypes.Insert(0, typeName);
                        _hasPerfectMatch = true;
                    } else
                    {
                        _displayTypes.RemoveAt(i);
                        _displayTypes.Insert(_hasPerfectMatch ? 1 : 0 ,typeName);
                    }
                } else if (!typeName.ToLower().Contains(currentTypeName))
                {
                    _hiddenTypes.Add(typeName);

                    _displayTypes.RemoveAt(i);
                }
            }
        }

        private void DelChars()
        {
            var currentTypeName = EditingString.ToLower();
            Debug.Log("currentTypeName: " + currentTypeName);
            for (int i = _hiddenTypes.Count - 1; i >= 0; i--)
            {
                var typeName = _hiddenTypes[i];

                if (string.Compare(currentTypeName, 0, typeName.ToLower(), 0, currentTypeName.Length) == 0)
                {
                    if (typeName.Length == currentTypeName.Length)
                    {
                        _hiddenTypes.RemoveAt(i);
                        _displayTypes.Insert(0, typeName);
                    }
                    else
                    {
                        _hiddenTypes.RemoveAt(i);
                        _displayTypes.Insert(_hasPerfectMatch ? 1 : 0, typeName);
                    }
                } else if (typeName.ToLower().Contains(currentTypeName))
                {
                    _displayTypes.Add(typeName);
                    _hiddenTypes.RemoveAt(i);
                }
            }
        }

        public void SetParaType(string typeName)
        {
            var para = _paraList[_helperIndex];
            para.ParaType = typeName;
            _paraList[_helperIndex] = para;
            Debug.Log("new Value: " + _paraList[_helperIndex].ParaType);
        }

        public string GetProtoName()
        {
            return _protoName;
        }

        public bool CheckData()
        {
            bool qualified = true;
            if (_protoName == "" || _protoName[0] == ' ' || _protoName == "NewProto")
            {
                qualified = false;
                Debug.Log("Please set proto name");
            }

            return qualified;
        }

        private void AdjustProtoName()
        {
            if (_protoName.Length > 0)
            {
                if (_protoName[0] >= 97 && _protoName[0] <= 122)
                {
                    string newValue = "";
                    newValue += (char)(_protoName[0] - 32);

                    for (int i = 1; i < _protoName.Length; i++)
                    {
                        newValue += _protoName[i];
                    }

                    _protoName = newValue;
                }
            }
            else
            {
                if (name != "" && name[0] != ' ')
                {
                    _protoName = name;
                }
                else
                {
                    _protoName = "NewProto";
                }
            }
        }

        public void SetSelectedValue(string selectedStr)
        {
            int controlCount = 0;

            if (ParaListVisible)
            {
                for (int i= 0; i < _paraList.Count; i++)
                {
                    if (controlCount == _helperIndex)
                    {

                        var para = _paraList[i];
                        para.ParaType = selectedStr;
                        _paraList[i] = para;
                        return;
                    }
                    controlCount++;
                }
            }
        }

        public void SetEditingValue()
        {
            int controlCount = 0;

                for (int i = 0; i < _paraList.Count; i++)
                {
                    if (controlCount == _helperIndex)
                    {

                        var para = _paraList[i];
                        EditingString = para.ParaType;
                        return;
                    }
                    controlCount++;
                }
        }

        public List<ParaInfo> GetParaList()
        {
            if(_paraList == null)
            {
                _paraList = new List<ParaInfo>();
            }
            return _paraList;
        }

        public void CopyData(ProtoConfig newconfig)
        {
            newconfig._protoName = _protoName;
            newconfig._protoType = _protoType;
            newconfig._paraList = _paraList;
            newconfig.name = _protoName;
            newconfig._language = _language;
        }
        
        public string GetProtoType()
        {
            return _protoType == ProtoType.Req ? "IReqProto" : "IRltProto";
        }
    }
}
#endif