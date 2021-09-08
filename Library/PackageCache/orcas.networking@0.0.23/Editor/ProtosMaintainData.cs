#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    [Serializable]
    public class ProtosMaintainData : ScriptableObject
    {
        [SerializeField]
        private List<string> _protoList;

        [SerializeField]
        private List<string> _luaProtoList;

        public ProtosMaintainData()
        {
            _protoList = new List<string>();
            _luaProtoList = new List<string>();
        }

        public List<string> GetProtoList()
        {
            var newList = new List<string>(_protoList);
            return newList;
        }

        public void AddProto(string protoName)
        {
           
            if (!_protoList.Contains(protoName))
            {
                _protoList.Add(protoName);
            }
        }

        public void DeleteProto(string protoName)
        {
            if (_protoList.Contains(protoName))
            {
                _protoList.Remove(protoName);
            }
        }

        public List<string> GetLuaProtoList()
        {
            var newList = new List<string>(_luaProtoList);
            return newList;
        }

        public void AddLuaProto(string protoName)
        {

            if (!_luaProtoList.Contains(protoName))
            {
                _luaProtoList.Add(protoName);
            }
        }

        public void DeleteLuaProto(string protoName)
        {
            if (_luaProtoList.Contains(protoName))
            {
                _luaProtoList.Remove(protoName);
            }
        }


    }
}
#endif