#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Orcas.Networking
{
    [Serializable]
    public class LuaProtoConfig_DataType
    {
        [SerializeField]
        public string Name;
      
        [SerializeField]
        public LuaTypeCode Type = LuaTypeCode.Int32;
      
        [SerializeField]
        public int[] Enums;
     
        [SerializeField]
        public LuaProtoConfig_DataType[] Table;

        /*
        public LuaProtoConfig_DataType(LuaProtoConfig_DataType data)
        {  
            this.Name = data.Name;
            this.Type = data.Type;
            this.Enums = (int[])data.Enums.Clone();
            this.Table = (LuaProtoConfig_DataType[])data.Table.Clone();
        }
        */
        public void RemoveItem(int index)
        {
            if (Table == null)
            {
                Debug.Log("Table is null! ");
                return;
            }
            Debug.Log("Table.length: " + Table.Length);
            var dataList = Table.ToList();
            dataList.RemoveAt(index);
            Table = dataList.ToArray();
        }

        public void AddNewItem()
        {
            Table = Table.Append(new LuaProtoConfig_DataType()).ToArray();
        }

        public void CheckTableCount(int count)
        {
            if (Table == null || Table.Length == count) return;

            if (Table.Length < count)
            {
                int diff = count - Table.Length;
                var emptyTables = new LuaProtoConfig_DataType[diff];
                var dataList = Table.ToList();
                dataList.AddRange(emptyTables);
                Table = dataList.ToArray();
            } else
            {
                int diff = Table.Length - count;
                var dataList = Table.ToList();
                dataList.RemoveRange(count, diff);
                Table = dataList.ToArray();
            }
            
        }
    }
}
#endif