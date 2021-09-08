#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace Orcas.Excel
{

    public class ExcelUtility 
    {
        private static void SetDefine(BuildTargetGroup group, string define)
        {
            var str = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var strs = str.Split(';');
            for (var i = 0; i < strs.Length; i++)
            {
                if(string.Equals(strs[i], define))
                {
                    return;
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, str + $";{define}");
        }
        
        [InitializeOnLoadMethod]
        public static void ComplierRun()
        {
            SetDefine(BuildTargetGroup.Android, "Core");
            SetDefine(BuildTargetGroup.iOS, "Core");
            SetDefine(BuildTargetGroup.Standalone, "Core");
        }

        /// <summary>
        /// 把Worksheet中的表格转化成csv字符串
        /// </summary>
        /// <param name="worksheet">Excel的Worksheet</param>
        /// <param name="start">开始地址的索引</param>
        /// <param name="end">结束地址的索引</param>
        /// <returns></returns>
        public static string ExcelTable2CsvString(ExcelWorksheet worksheet, ExcelCellAddress start,
            ExcelCellAddress end)
        {
            // 创建一个StringBuilder存储数据
            StringBuilder stringBuilder = new StringBuilder ();
            int startRow = start.Row;
            int startColumn = start.Column;
            int endRow = end.Row;
            int endColumn = end.Column;
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    stringBuilder.Append (worksheet.Cells[i, j].Value);
                    if(j != endColumn){
                        stringBuilder.Append(",");
                    }
                }
                stringBuilder.Append ("\r\n");
            }
            return stringBuilder.ToString();
        }

        public static string CsList2LuaListJson(List<Dictionary<string, object>> list, string key = "ID")
        {
            Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item.ContainsKey(key))
                {
                    string keyValue = item[key].ToString();
                    if (dict.ContainsKey(keyValue) == false)
                    {
                        dict.Add(keyValue, item);
                    } 
                }
            }
            return JsonConvert.SerializeObject(dict);
        }
        
        /// <summary>
        /// 把Worksheet中的表格转化成csv字符串
        /// </summary>
        /// <param name="worksheet">Excel的Worksheet</param>
        /// <param name="tableIndex">第几个表格</param>
        /// <returns></returns>
        public static string ExcelTable2CsvString(ExcelWorksheet worksheet, int tableIndex)
        {
            if (tableIndex < 0 || tableIndex >= worksheet.Tables.Count)
            {
                return default;
            }

            var table = worksheet.Tables[tableIndex];
            return ExcelTable2CsvString(worksheet, table.Address.Start, table.Address.End);
        }

        /// <summary>
        /// 加载Excel
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <returns></returns>
        public static ExcelPackage LoadExcelFile(string excelFilePath)
        {
            FileInfo output = new FileInfo(excelFilePath);
            ExcelPackage ep =new ExcelPackage(output);
            return ep;
        }

        /// <summary>
        /// 通过名字获取Worksheet
        /// </summary>
        /// <param name="package"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ExcelWorksheet GetWorksheetByName(ExcelPackage package, string name)
        {
            for (int i = 0; i < package.Workbook.Worksheets.Count; i++)
            {
                //默认读取第一个数据表
                ExcelWorksheet worksheet = package.Workbook.Worksheets [i+1];
                if (worksheet.Name == name)
                {
                    return worksheet;
                }
            }
            return default;
        }                       
    }
}
#endif
