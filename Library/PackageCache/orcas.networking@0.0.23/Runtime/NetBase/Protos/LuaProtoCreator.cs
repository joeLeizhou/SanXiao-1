#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
namespace Orcas.Networking
{

    public class LuaProtoCreator
    {
        public static void Create(LuaProtoConfig config)
        {
            CheckTargetPath();
            CreateProto(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CheckTargetPath()
        {
            if (!Directory.Exists(NetworkResourcePath.LuaProtoCreator_targetPath))
            {
                Directory.CreateDirectory(NetworkResourcePath.LuaProtoCreator_targetPath);
            }
        }

        private static void CreateProto(LuaProtoConfig config)
        {
            var protoName = config.GetProtoName();
          
            var protoDataArray = config.GetProtoDataArray();
            GenerateFile(protoName, protoDataArray);
        }

        private static string GetFilePath(string protoName)
        {
            var filePath = NetworkResourcePath.LuaProtoCreator_targetPath + "/" + protoName + ".lua";

            if (Directory.Exists(filePath))
            {
                AssetDatabase.DeleteAsset(filePath);
            }

            return filePath;
        }

        private static void GenerateFile(string protoName, LuaProtoConfig_DataType[] protoDataArray)
        {
            var filePath = GetFilePath(protoName);
            var lines = new List<string>();
            lines.Add(DefineLuaProtoTable(protoName));
            lines.AddRange(LuaProtoTableConstructor(protoName, protoDataArray));

            
            using (StreamWriter outfile =
                    new StreamWriter(filePath))
            {
                foreach(var line in lines)
                {
                    outfile.WriteLine(line);
                }
            }
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }

        private static string DefineLuaProtoTable(string protoName)
        {
            string defineStr = "local " + protoName + " = {}";
            return defineStr;
        }
       
        private static List<string> LuaProtoTableConstructor(string protoName, LuaProtoConfig_DataType[] protoDataArray)
        {
            var res = new List<string>();
            var left = protoName + ".Table = {";

            var tableLines = HandleTables(protoDataArray, 1);

            var endLines = HandleEnd(protoName);

            res.Add(left);
            res.AddRange(tableLines);
            res.AddRange(endLines);

            return res;
        }

        private static List<string> HandleLuaProtoData(LuaProtoConfig_DataType data, int indentLevel, bool isLastItem = false)
        {
            var strs = new List<string>();
           
            var dataName = data.Name;
            var dataType = data.Type;
            if (dataType == LuaTypeCode.Empty)
            {
                var beginLine = BeginTable(indentLevel);
                var tableLines = HandleTables(data.Table, indentLevel + 1);
                var endLine = EndTable(indentLevel) + (isLastItem ? "" : AddComma());
                strs.Add(beginLine);
                strs.AddRange(tableLines);
                strs.Add(endLine);
            }
            else if (IsSingleLineType(dataType))
            {
                var str = BeginTable(indentLevel);
                str += HandleName(dataName);
                str += AddComma();
                str += HandleType(dataType);

                if(data.Enums.Length > 0)
                {
                    str += AddComma();
                    str += HandleEnum(data.Enums);             
                }

                str += EndTable() + (isLastItem ? "" : AddComma());

                strs.Add(str);
            }
            else
            {
                var str = BeginTable(indentLevel);
                str += HandleName(dataName);
                str += AddComma();
                str += HandleType(dataType);
                str += AddComma();

                str += "Table = " + BeginTable();

                var tableLines = HandleTables(data.Table, indentLevel + 1);

                var endStr = EndTable(indentLevel) + EndTable() + (isLastItem ? "" : AddComma());

                strs.Add(str);
                strs.AddRange(tableLines);
                strs.Add(endStr);
            }

            return strs;
        }

        private static string HandleName(string dataName)
        {
            var str = "Name = " + AddQuotation() + dataName + AddQuotation();

            return str;
        }

        private static string HandleType(LuaTypeCode type)
        {
            var str = "Type = TypeCode." + type;

            return str;
        }

        private static string HandleEnum(int[] dataEnum)
        {
            var str = "Enums = " + BeginTable();

            for(int i = 0; i < dataEnum.Length; i++)
            {
                if (i != 0) str += AddComma();
                str += dataEnum[i];
            }

            str += EndTable();

            return str;
        }

        private static List<string> HandleTables(LuaProtoConfig_DataType[] tables, int indentLevel)
        {
            var strs = new List<string>();

            for(int i = 0; i < tables.Length; i++)
            {
                var table = tables[i];
                
                var lines = new List<string>();
           
                if(i != tables.Length - 1)
                    lines.AddRange(HandleLuaProtoData(table, indentLevel));
                else
                   lines.AddRange(HandleLuaProtoData(table, indentLevel, true));

                strs.AddRange(lines);
            }

            return strs;
        }

        private static string AddComma()
        {
            return ", ";
        }    


        private static string BeginTable(int indentLevel = 0)
        {
            return AddIndent(indentLevel) + "{";
        }

        private static string EndTable(int indentLevel = 0)
        {
            return AddIndent(indentLevel) + "}";
        }

        private static string AddQuotation()
        {
            return "\"";
        }

        private static string AddIndent(int indentLevel)
        {
            string indent = "";
            for(int i = 0; i < indentLevel; i++)
            {
                indent += "     ";
            }

            return indent;
        }

        private static bool IsSingleLineType(LuaTypeCode type)
        {
            switch (type)
            {
                case LuaTypeCode.Object:
                case LuaTypeCode.BasicArray:
                case LuaTypeCode.CustomArray:
                case LuaTypeCode.VariantArray:
                    return false;
                default:
                    return true;
            }
        }

        private static List<string> HandleEnd(string protoName)
        {
            var endStr = new List<string>();
            endStr.Add("}");
            endStr.Add(" ");
            endStr.Add("return " + protoName);
            return endStr;
        }

        
    }
}
#endif