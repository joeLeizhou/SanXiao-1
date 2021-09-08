#if UNITY_EDITOR
using Orcas.Networking;
using Orcas.Core.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class LuaProtoConfig : ScriptableObject
{
    [SerializeField]
    private string _protoName;
    [SerializeField]
    private LuaProtoConfig_DataType[] _protoDataArray;

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

    public string GetProtoName()
    {
        return _protoName;
    }

    public void CopyData(LuaProtoConfig newConfig)
    {
        newConfig.name = _protoName;
        newConfig._protoName = _protoName;
        newConfig._protoDataArray = _protoDataArray;
    }

    public LuaProtoConfig_DataType[] GetProtoDataArray()
    {
        //return (LuaProtoConfig_DataType[])_protoDataArray.Clone();
        return _protoDataArray;
    }

    public void AddNewItem()
    {
        //return (LuaProtoConfig_DataType[])_protoDataArray.Clone();
        _protoDataArray = _protoDataArray.Append(new LuaProtoConfig_DataType()).ToArray();
    }

    public void RemoveItem(int index)
    {
        var arrayList = _protoDataArray.ToList();
        arrayList.RemoveAt(index);
        _protoDataArray = arrayList.ToArray();
    }

}
#endif