#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace Orcas.Networking
{
    public class ProtoCreator
    {
        public static void Create(ProtoConfig config)
        {
            CheckTargetPath();
            CreateProto(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CheckTargetPath()
        {
            if (!Directory.Exists(NetworkResourcePath.CSProtoCreator_targetPath))
            {
                Directory.CreateDirectory(NetworkResourcePath.CSProtoCreator_targetPath);
            }
        }

        private static void CreateProto(ProtoConfig config)
        {
            var protoName = config.GetProtoName();
            var paraList = config.GetParaList();
            var protoType = config.GetProtoType();
            GenerateFile(protoName, protoType, paraList);
        }

        private static string GetFilePath(string protoName)
        {
            var filePath = NetworkResourcePath.CSProtoCreator_targetPath + "/" + protoName + ".cs";

            if (Directory.Exists(filePath))
            {
                AssetDatabase.DeleteAsset(filePath);
            }

            return filePath;
        }

        private static List<string> GetRequiredNameSpaces(List<ProtoConfig.ParaInfo> paraList)
        { 
            var requiredNamespaces = GetDefaultNameSpaces();
            var paraNameSpaces = GetParaNameSpaces(paraList);
            foreach (var paraNameSpace in paraNameSpaces)
            {
                if (!requiredNamespaces.Contains(paraNameSpace))
                    requiredNamespaces.Add(paraNameSpace);
            }
            return requiredNamespaces;
        }

        private static List<string> GetDefaultNameSpaces()
        {
            var defaultNameSpaces = new List<string>();
            defaultNameSpaces.Add("Orcas.Networking");
            defaultNameSpaces.Add("Orcas.Networking.Tcp");
            defaultNameSpaces.Add("System.Collections");
            defaultNameSpaces.Add("System.Collections.Generic");
            defaultNameSpaces.Add("System");
            return defaultNameSpaces;
        }

        private static List<string> GetParaNameSpaces(List<ProtoConfig.ParaInfo> paraList)
        {
            var paraNamsSpaces = new List<string>(); 
            foreach(var paraInfo in paraList)
            {
                var typeStr = paraInfo.ParaType;
                var nameSpace = GetNamespaceFromTypeStr(typeStr);
                if(nameSpace != "" && !string.IsNullOrEmpty(nameSpace) && !paraNamsSpaces.Contains(nameSpace))
                {
                    Debug.Log("paraNameSpace: " + nameSpace);
                    paraNamsSpaces.Add(nameSpace);
                }
            }
            return paraNamsSpaces;
        }

        private static string GetNamespaceFromTypeStr(string typeStr)
        {
            Assembly CSAssembly = Assembly.Load("Assembly-CSharp");
            foreach (var type in CSAssembly.GetTypes())
            {
                if (type.IsPublic && type.FullName == typeStr)
                {
                    return type.Namespace;    
                }
            }

            return string.Empty;
        }

        private static void GenerateFile(string protoName, string protoType, List<ProtoConfig.ParaInfo> paraList)
        {
            var filePath = GetFilePath(protoName);
            var requiredNameSpaces = GetRequiredNameSpaces(paraList);

            var usingPart = CreateUsingPart(requiredNameSpaces);
            var namePart = CreateClassNamePart(protoName, protoType);
            var paraPart = CreateParasPart(paraList);
            var funcPart = CreateFuncPart(protoType);

            using (StreamWriter outfile =
                    new StreamWriter(filePath))
            {
                foreach (var item in usingPart)
                {
                    outfile.WriteLine(item);
                }

                outfile.WriteLine(" ");
                outfile.WriteLine(namePart);
                outfile.WriteLine("{");
                foreach (var item in paraPart)
                {
                    outfile.WriteLine("     " + item);
                }
                outfile.WriteLine(" ");
                foreach (var item in funcPart)
                {
                    outfile.WriteLine("     " + item);
                }
                outfile.WriteLine("} ");
            }
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }

        private static List<string> CreateUsingPart(List<string> requiredNameSpaces)
        {
            var res = new List<string>();
            foreach(var requiredNameSpace in requiredNameSpaces)
            {
                res.Add("using " + requiredNameSpace + ";");
            }
            return res;
        }

        private static string CreateClassNamePart(string protoName, string protoType)
        {
            var res = "public class " + protoName + " : " + protoType;
            return res;
        }

        private static List<string> CreateParasPart(List<ProtoConfig.ParaInfo> paraList)
        {
            var res = new List<string>();
            res.Add("public ushort ID { get; set; }");
            for(int i = 0; i < paraList.Count; i++)
            {
                var paraInfo = paraList[i];
                if (paraInfo.ParaType == "" || paraInfo.ParaType[0] == ' ') continue;
                if (paraInfo.ParaName == "" || paraInfo.ParaName[0] == ' ')
                {
                    paraInfo.ParaName = "para" + i;
                }
               res.Add("public " + paraInfo.ParaType + " " + paraInfo.ParaName + ";");
            }
            return res;
        }

        private static List<string> CreateFuncPart(string protoType)
        {
            var res = new List<string>();
            if (protoType == "IRltProto")
            {
                res.Add("public void Deal()");
                res.Add("{");
                res.Add(" ");
                res.Add("}");              
            }
            return res;
        }
    }
}
#endif