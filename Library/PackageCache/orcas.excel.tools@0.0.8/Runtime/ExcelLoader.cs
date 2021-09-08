using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;
using Orcas.Resources;
using UnityEngine;

namespace Orcas.Excel
{
    public class ExcelLoader<TKey> where TKey : IConvertible
    {
        private static readonly Dictionary<Type, Dictionary<TKey, IExcelData<TKey>>> ExcelMap =
            new Dictionary<Type, Dictionary<TKey, IExcelData<TKey>>>();
        
        public static readonly List<IConverter> Converters = new List<IConverter>()
        {
            new IntConverter(),
            new BoolConverter(),
            new FloatConverter(),
            new StringConverter(),
            new FloatArrConverter(),
            new IntArrConverter(),
            new MaskConverter(),
            new StringArrConverter(),
            new BoolArrConverter(),
            new IntArrArrConverter(),
        };
        
        public static T GetExcelData<T>(TKey id) where T : IExcelData<TKey>
        {
            var type = typeof(TKey);
            if (ExcelMap[type].ContainsKey(id)) return (T)ExcelMap[type][id];
            Debug.LogError("Csv " + typeof(T) + " not ContainsKey " + id);
            return default;
        }
        
        public static Dictionary<TKey, IExcelData<TKey>> Import<T>(string excelFilePath, string text = null) where T : IExcelData<TKey>, new()
        {
            var type = typeof(T);
            var typeKey = typeof(TKey);
            byte[] bytes = null;
            if (string.IsNullOrEmpty(text))
            {
                var textAsset = ResourceLoaderManager.AssetLoader.LoadOriginalAsset<TextAsset>(excelFilePath);
                if (textAsset != null) bytes = textAsset.bytes;
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(text);
            }
            
            if (bytes == null)
            {
                Debug.LogError($"找不到excel文件 {excelFilePath}");
                return null;
            }

            var stream = new MemoryStream(bytes);
            var ep =new ExcelPackage(stream);
            var worksheets = ep.Workbook.Worksheets;
            ExcelWorksheet workSheet = null;
            for (var i = 0; i < worksheets.Count; i++)
            {
                if (!string.Equals(worksheets[i].Name, type.Name, StringComparison.Ordinal)) continue;
                workSheet = worksheets[i];
                break;
            }

            if (workSheet == null)
            {
                Debug.LogError($"找不到数据表 {type.Name}");
                return null;
            }

            var col = workSheet.Dimension.Columns;
            var row = workSheet.Dimension.Rows;
            if (row < 2)
            {
                Debug.LogError($"请检查表格是否符合规范（第一行变量名，第二行变量类型） {typeof(T).Name}");
                return null;
            }
            
            var table = new Dictionary<TKey, IExcelData<TKey>>();
            if (ExcelMap.ContainsKey(typeKey))
            {
                ExcelMap.Remove(typeKey);
            }
            ExcelMap.Add(typeKey, table);
            var props = type.GetProperties();
            var propsCol = new int[props.Length];
            var varType = new string[props.Length];
            for (var i = 0; i < props.Length; i++)
            {
                for (var j = 1; j <= col; j++)
                {
                    if (!string.Equals(workSheet.GetValue(1, j).ToString(), props[i].Name,
                        StringComparison.Ordinal)) continue;
                    propsCol[i] = j;
                    varType[i] = workSheet.GetValue(2, j).ToString();
                    break;
                }
            }

            for (var i = 4; i <= row; i++)
            {
                var data = new T();
                for (var j = 0; j < props.Length; j++)
                {
                    if (propsCol[j] != 0)
                    {
                        var converter = Converters.Find((a) => a.Key == varType[j]);
                        try
                        {
                            var value = workSheet.GetValue(i, propsCol[j]).ToString();
                            props[j].SetValue(data, converter.Covert2Value(value));
                        }
                        catch(Exception e)
                        {
                            Debug.LogError("[Csv] Error Format: " + " row " + i + "col " + j + ":" + varType[j] + " " + e);
                        }
                    }
                }
                if (table.ContainsKey(data.ID) == false)
                {
                    table.Add(data.ID, data);
                }
            }
            
            return table;
        }
        
        
    }
}
