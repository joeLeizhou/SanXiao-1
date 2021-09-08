using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using UnityEngine;
using Newtonsoft.Json;

namespace Orcas.Data
{
    /// <summary>
    /// 持久化存储器
    /// 只能存一种类型的变量，或者是个父节点
    /// 可以监听数据变化
    /// </summary>
    public sealed class Storage
    {
        private readonly Dictionary<string, Storage> _children;
        private int _intValue;
        private float _floatValue;
        private string _stringValue;
        private StorageType _type;
        private Action<Storage, Storage> _callBack;

        /// <summary>
        /// 存取整形
        /// </summary>
        public int IntValue
        {
            set
            {
                if (_type == StorageType.Node)
                {
                    Debug.LogError("[Storage] 对象节点不能存值");
                    return;
                }

                _type = StorageType.Int;
                _intValue = value;
                PlayerPrefs.SetInt(FullName, _intValue);
                OnValueChanged(this, this);
            }
            get => _type == StorageType.Int ? _intValue : int.MinValue;
        }

        /// <summary>
        /// 存取浮点数
        /// </summary>
        public float FloatValue
        {
            set
            {
                if (_type == StorageType.Node)
                {
                    Debug.LogError("[Storage] 对象节点不能存值");
                    return;
                }

                _type = StorageType.Float;
                _floatValue = value;
                PlayerPrefs.SetFloat(FullName, _floatValue);
                OnValueChanged(this, this);
            }
            get => _type == StorageType.Float ? _floatValue : float.MinValue;
        }

        /// <summary>
        /// 存取字符串
        /// </summary>
        public string StringValue
        {
            set
            {
                if (_type == StorageType.Node)
                {
                    Debug.LogError("[Storage] 对象节点不能存值");
                    return;
                }

                _type = StorageType.String;
                _stringValue = value;
                PlayerPrefs.SetString(FullName, _stringValue);
                OnValueChanged(this, this);
            }
            get => _type == StorageType.String ? _stringValue : null;
        }

        public readonly string FullName;
        public readonly string FatherPath;
        public readonly string Name;
        private Storage _father;

        internal Storage(string fatherPath, string name)
        {
            Name = name;
            FatherPath = fatherPath;
            FullName = fatherPath != null ? $"{fatherPath}.{name}" : name;
            _type = StorageType.None;
            _intValue = PlayerPrefs.GetInt(FullName, int.MinValue);
            _floatValue = PlayerPrefs.GetFloat(FullName, float.MinValue);
            _stringValue = PlayerPrefs.GetString(FullName, string.Empty);
            if (string.IsNullOrEmpty(_stringValue) == false)
            {
                _type = StorageType.String;
            }
            else if (_intValue != int.MinValue)
            {
                _type = StorageType.Int;
            }
            else if (Math.Abs(_floatValue - float.MinValue) > 0.001f)
            {
                _type = StorageType.Float;
            }

            _children = new Dictionary<string, Storage>();
            _father = null;
        }

        internal Storage GetChild(string path)
        {
            var names = path.Split(' ', ',', '.');
            return GetChild(names);
        }

        internal Storage GetChild(params string[] names)
        {
            if (names == null) return null;
            var storage = this;
            var fName = new StringBuilder(FullName);
            for (var i = 0; i < names.Length; i++)
            {
                if (storage._children.ContainsKey(names[i]) == false)
                {
                    storage._children[names[i]] =
                        new Storage(fName.ToString(), names[i]) {_father = storage};
                }

                storage._type = StorageType.Node;
                storage = storage._children[names[i]];
                fName.Append('.');
                fName = fName.Append(names[i]);
            }

            return storage;
        }

        public void AddListener(Action<Storage, Storage> callBack)
        {
            _callBack += callBack;
        }

        public void RemoveListener(Action<Storage, Storage> callBack)
        {
            if (callBack != null)
            {
                _callBack -= callBack;
            }
        }

        public string GetJsonData()
        {
            if (_type == StorageType.None) return null;
            var sb = new StringBuilder();
            sb.Append($"{{\"t\":{(byte) _type}");
            switch (_type)
            {
                case StorageType.Int:
                    sb.Append($",\"v\":{_intValue}");
                    break;
                case StorageType.Float:
                    sb.Append($",\"v\":{_floatValue}");
                    break;
                case StorageType.String:                                        
                    string strJson = JsonConvert.SerializeObject(_stringValue);                    
                    sb.Append($",\"v\":{strJson}");
                    break;
                case StorageType.Node:
                    sb.Append($",\"v\":{{");
                    var c = 0;
                    var sc = _children.Count;
                    foreach (var kv in _children)
                    {
                        var data = kv.Value.GetJsonData();
                        c++;
                        if (data != null)
                        {
                            sb.Append("\"" + kv.Key + "\"");
                            sb.Append(':');
                            sb.Append(kv.Value.GetJsonData());
                            if (c < sc)
                            {
                                sb.Append(',');
                            }
                        }
                        else{
                            // 如果最后一项是空，则会多加一个逗号
                            if(c == sc){
                                if (sb.Length > 0 && sb[sb.Length - 1] == ',')
                                {
                                    sb.Remove(sb.Length - 1, 1);
                                }
                            }
                        }
                    }
                    sb.Append('}');
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            sb.Append('}');
            return sb.ToString();
        }

        internal void SaveToLocal()
        {
            switch (_type)
            {
                case StorageType.Int:
                    PlayerPrefs.SetInt(FullName, _intValue);
                    break;
                case StorageType.Float:
                    PlayerPrefs.SetFloat(FullName, _floatValue);
                    break;
                case StorageType.String:
                    PlayerPrefs.SetString(FullName, _stringValue);
                    break;
                case StorageType.Node:
                {
                    foreach (var kv in _children)
                    {
                        kv.Value.SaveToLocal();
                    }

                    break;
                }
            }
        }

        internal void DeleteFromLocal()
        {
            _callBack = null;
            switch (_type)
            {
                case StorageType.Int:
                case StorageType.Float:
                case StorageType.String:
                    PlayerPrefs.DeleteKey(FullName);
                    break;
                case StorageType.Node:
                {
                    foreach (var kv in _children)
                    {
                        kv.Value.DeleteFromLocal();
                    }

                    break;
                }
            }
        }
        private void OnValueChanged(Storage current, Storage sender)
        {
            _callBack?.Invoke(current, sender);
            current._father?.OnValueChanged(current._father, sender);
        }
    }
}