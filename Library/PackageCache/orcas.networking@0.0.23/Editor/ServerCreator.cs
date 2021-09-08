#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Orcas.Networking
{

    public class ServerCreator
    {  
        public static void Create(ServerConfig config)
        {

            if (!CheckServerName(config))
                return;
            
            CheckTargetPath();
            
            CreateCSFile(config);
            CreateLuaFile(config);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateServer_CS(ServerConfig config)
        {
            if (!CheckServerName(config))
                return;
            CheckTargetPath();
            CreateCSFile(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateServer_Lua(ServerConfig config)
        {
            if (!CheckServerName(config))
                return;
            CheckTargetPath();
            CreateLuaFile(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static bool CheckServerName(ServerConfig config)
        {
            if (config.GetServerName() == "" || config.GetServerName() == "UnNamedServer")
            {
                Debug.Log("Create ServerGenerator Error!--- Please input ServerName.");
                return false;
            }

            return true;
        }

        public static void CheckTargetPath()
        {
            if (!Directory.Exists(NetworkResourcePath.ServerCreator_targetPath))
            {
                Directory.CreateDirectory(NetworkResourcePath.ServerCreator_targetPath);
            }
        }

        public static void CreateCSFile(ServerConfig config)
        {
            var serverName = config.GetServerName();
            var serverDir = GetServerDir(serverName);

            var filePath = serverDir + "/" + serverName + ".cs";
          
            if (Directory.Exists(filePath))
            {
                AssetDatabase.DeleteAsset(filePath);
            }

            var decodeMethod = config.GetDecodeMethod();
            string decoderInitStr;
            int decoderClassNameLength = 0;
            bool decoderHasANameSpace = false;
            int decoderNameLength = 0;
            var decoderNameSpace = "";
            if (decodeMethod == ServerConfig.DecodeMethod.Default)
            {
                decoderInitStr = "new DefaultDecoder();";
            }
            else
            {
                decoderInitStr = "new " + config._decoder.name + "();";
                decoderClassNameLength = config.CustomDecoderClassName.Length;
                decoderNameLength = config._decoder.name.Length;
                decoderHasANameSpace = (decoderClassNameLength != decoderNameLength);
            }

            if (decoderHasANameSpace)
            {
                for (int i = 0; i < decoderClassNameLength - decoderNameLength - 1; i++)
                {
                    decoderNameSpace += config.CustomDecoderClassName[i];
                }
            }


            var apiList = config.GetApiList();
            List<string> apiDelcareStrs = new List<string>();
            List<string> apiConfigStrs = new List<string>();

            for (int i = 0; i < apiList.Count; i++)
            {
                var apiInfo = apiList[i];
                if (apiInfo.GetPath().Length == 0) continue;
                if (apiInfo.GetFormat() == APIInfo.Format.Lua) continue;
                var apiClassName = APICreator.CreateClassName(apiInfo.GetPath(), serverName);

                var apiDeclareStr = "public " + apiClassName + " " + apiClassName + ";";
                apiDelcareStrs.Add(apiDeclareStr);

                var apiConfigStr = apiClassName + " =  new " + apiClassName + "(" + serverName + "Config, " + i + ");";
                apiConfigStrs.Add(apiConfigStr);
            }

            using (StreamWriter outfile =
                     new StreamWriter(filePath))
            {
                outfile.WriteLine("using UnityEngine;");
                outfile.WriteLine("using System.IO;");
                outfile.WriteLine("using System.Collections;");
                outfile.WriteLine("using System.Collections.Generic;");
                outfile.WriteLine("using Orcas.Networking;");
                outfile.WriteLine("#if UNITY_EDITOR");
                outfile.WriteLine("     using UnityEditor;");
                outfile.WriteLine("#endif");
                if (decoderHasANameSpace)
                {
                    outfile.WriteLine("using " + decoderNameSpace + ";");
                }

                outfile.WriteLine(" ");
                outfile.WriteLine(" public class " + serverName + ": HttpServerBase");
                outfile.WriteLine("{");
                outfile.WriteLine("     " + "public string ServerConfigPath = \"" + NetworkResourcePath.ServerConfigDir + "/" + serverName + ".asset\";");
                outfile.WriteLine("     " + "public ServerConfig " + serverName + "Config" + "{ get; private set; }");
               
                for (int i = 0; i < apiDelcareStrs.Count; i++)
                {
                    outfile.WriteLine("     " + apiDelcareStrs[i]);
                }

                outfile.WriteLine("     " + "private static " + serverName + " _instance;");
                outfile.WriteLine("     " + "public static " + serverName + " Instance");
                outfile.WriteLine("     " + "{");
                outfile.WriteLine("     " + "   get");
                outfile.WriteLine("     " + "   {");
                outfile.WriteLine("     " + "   " + "   " + "if (_instance == null) _instance = new " + serverName + "();");
                outfile.WriteLine("     " + "   " + "   " + "return _instance;");
                outfile.WriteLine("     " + "   }");
                outfile.WriteLine("     " + "}");

                outfile.WriteLine(" ");
                outfile.WriteLine("     " + "public " + serverName + "()");
                outfile.WriteLine("     " + "{");
                outfile.WriteLine("         " + "#if UNITY_EDITOR");

            

                outfile.WriteLine("             " + serverName + "Config = AssetDatabase.LoadAssetAtPath<ServerConfig>(ServerConfigPath);");
                outfile.WriteLine("         " + "#else");
                outfile.WriteLine("             " + serverName + "Config = Resources.Load<ServerConfig>(ServerConfigPath);");
                outfile.WriteLine("         " + "#endif");
                outfile.WriteLine(" ");

                outfile.WriteLine("         " + "DefaultUrl = " + serverName + "Config.GetDefaultUrlInfo().GetUrl();");
                outfile.WriteLine("         " + "Decoder = " + decoderInitStr);
                outfile.WriteLine("         " + "InitAPI();");
                outfile.WriteLine("     " + "}");

                outfile.WriteLine(" ");
                outfile.WriteLine("     " + "private void InitAPI()");
                outfile.WriteLine("     " + "{");

                for (int i = 0; i < apiConfigStrs.Count; i++)
                {
                    outfile.WriteLine("     " + "   " + apiConfigStrs[i]);
                }

                outfile.WriteLine("     " + "}");

                outfile.WriteLine(" ");

                outfile.WriteLine("     " + "public void ResetConfig(ServerConfig serverConfig)");
                outfile.WriteLine("     {");
                outfile.WriteLine("     " + "   " + serverName + "Config = serverConfig;");
                outfile.WriteLine("     " + "   InitAPI();");
                outfile.WriteLine("     }");
                outfile.WriteLine("}");
            }
        }

        public static void CreateLuaFile(ServerConfig config)
        {
            var serverName = config.GetServerName();
            var serverDir = GetServerDir(serverName);

            var filePath = serverDir + "/" + serverName + ".lua";

            if (Directory.Exists(filePath))
            {
                AssetDatabase.DeleteAsset(filePath);
            }


            var definitionStrs = CreateLuaServer_Definitions(serverName);
            //var setRequiresStrs = CreateLuaServerFunc_SetRequires(config);
            var initStrs = CreateLuaServerFunc_new(config);
            var initAPIStrs = CreateLuaServerFunc_InitAPI(config);
            var resetFuncStrs = CreateLuaServerFunc_ResetConfig(config);
            var orderStrs = CreateLuaServer_Orders(serverName);

            var contentList = new List<List<string>>();
            contentList.Add(definitionStrs);
            //contentList.Add(setRequiresStrs);
            contentList.Add(initStrs);
            contentList.Add(initAPIStrs);
            contentList.Add(resetFuncStrs);
            contentList.Add(orderStrs);

            using (StreamWriter outfile =
                     new StreamWriter(filePath))
            {
                for(int i = 0; i < contentList.Count; i++)
                {
                    var content = contentList[i];

                    for (int j = 0; j < content.Count; j++) {
                        outfile.WriteLine(content[j]);
                    }

                    outfile.WriteLine(" ");
                }
            }
        }

        private static List<string> CreateLuaServerFunc_new(ServerConfig config)
        {
            var serverName = config.GetServerName();
            
            bool useDefaultDecoder = config.GetDecodeMethod() == ServerConfig.DecodeMethod.Default;
            // var callRequireStr = "self.SetRequires()";
            var serverBaseStr = "local serverBase = getmetatable(self).__index";
            var decoderInitStr = "serverBase.Decoder = " + (useDefaultDecoder ? "Orcas.Networking.DefaultDecoder.New()" : config.CustomDecoderClassName + ".New();");
            var setServerConfigPathStr = "self.ServerConfigPath = \"" + NetworkResourcePath.ServerConfigDir + "/" + serverName + ".asset\"";
            var setServerConfigStr = "self." + serverName + "Config = self.GetConfigFromPath(serverBase, self.ServerConfigPath)";
            var getUrlInfoStr = "local urlInfo = self." + serverName + "Config.GetDefaultUrlInfo(self." + serverName + "Config)";
            var setDefaultUrlStr = "serverBase.DefaultUrl = urlInfo.GetUrl(urlInfo)";
            var initAPIStr = "self:InitAPI()";
            var returnStr = "return self";

            var funcBody = new List<string>();
            // funcBody.Add(callRequireStr);
            funcBody.Add(serverBaseStr);
            funcBody.Add(decoderInitStr);
            funcBody.Add(setServerConfigPathStr);
            funcBody.Add(setServerConfigStr);
            funcBody.Add(getUrlInfoStr);
            funcBody.Add(setDefaultUrlStr);
            funcBody.Add(initAPIStr);
            funcBody.Add(returnStr);

            var funcHead = CreateLuaHelper.CreateLuaFunctionHead(serverName, "new");

            return CreateLuaHelper.CreateLuaFunction(funcHead, funcBody);
        }

        private static List<string> CreateLuaServerFunc_SetRequires(ServerConfig config)
        {
            var serverName = config.GetServerName();
            var funcHead = CreateLuaHelper.CreateLuaFunctionHead(serverName, "SetRequires");

            var serverDir = GetServerDir(serverName);
            var apiList = config.GetApiList();
            List<string> requireStrs = new List<string>();

            for (int i = 0; i < apiList.Count; i++)
            {
                var apiInfo = apiList[i];
                if (apiInfo.GetPath().Length == 0 || apiInfo.GetFormat() == APIInfo.Format.CSharp) continue;
                
                var apiClassName = APICreator.CreateClassName(apiList[i].GetPath(), serverName);
                var requireStr = "require(\"" + serverDir + "/API/" + apiClassName + "\")";
                requireStrs.Add(requireStr);
            }

            return CreateLuaHelper.CreateLuaFunction(funcHead, requireStrs);
        }

        private static List<string> CreateLuaServerFunc_InitAPI(ServerConfig config)
        {
            var serverName = config.GetServerName();
            var funcHead = CreateLuaHelper.CreateLuaFunctionHead(serverName, "InitAPI");

            var serverDir = GetServerDir(serverName);
            var apiList = config.GetApiList();
            List<string> apiConfigStrs = new List<string>();

            for (int i = 0; i < apiList.Count; i++)
            {
                var apiInfo = apiList[i];
                if (apiInfo.GetPath().Length == 0) continue;
                if (apiInfo.GetFormat() == APIInfo.Format.CSharp) continue;
                var apiClassName = APICreator.CreateClassName(apiInfo.GetPath(), serverName);
                // var apiConfigStr = "self." + apiClassName + " = " + apiClassName + ":new(" + "self." + serverName + "Config, " + i + ", getmetatable(self).__index)";
                var apiDefineStr = "self." + apiClassName + " = require(\"" + serverDir + "/API/" + apiClassName + "\")";
                var apiConfigStr = "self." + apiClassName + ":new(" + "self." + serverName + "Config, " + i + ", getmetatable(self).__index)";
                apiConfigStrs.Add(apiDefineStr);
                apiConfigStrs.Add(apiConfigStr);
            }
           
            return CreateLuaHelper.CreateLuaFunction(funcHead, apiConfigStrs);
        }

        private static List<string> CreateLuaServer_Definitions(string serverName)
        {
            var defineServerBaseStr = "local httpServerBase = Orcas.Networking.HttpServerBase.New()";
            var defineServerStr = "local " + serverName + " = {}";
            var setMetaTableStr = "setmetatable(" + serverName + ", {__index = httpServerBase})";
        
            var orders = new List<string>();
            orders.Add(defineServerBaseStr);
            orders.Add(defineServerStr);
            orders.Add(setMetaTableStr);
   
            return orders;
        }
        private static List<string> CreateLuaServer_Orders(string serverName)
        {
            var returnStr = "return " + serverName + ":new()";       
            var orders = new List<string>();
            orders.Add(returnStr);

            return orders;
        }

        private static List<string> CreateLuaServerFunc_ResetConfig(ServerConfig config)
        {
            var serverName = config.GetServerName();
            var arguments = new List<string>();
            arguments.Add("serverConfig");
            
            var funcHead = CreateLuaHelper.CreateLuaFunctionHead(serverName, "ResetConfig", arguments);
           
            var setStrs = new List<string>();

            var resetConfigStr = "self." + serverName + "Config = serverConfig";
            var initAPIStr = "self.InitAPI()";
            setStrs.Add(resetConfigStr);
            setStrs.Add(initAPIStr);

            return CreateLuaHelper.CreateLuaFunction(funcHead, setStrs);
        }

        public static string CreateClassName(string serverName)
        {
            string suffix = "";
            return serverName + suffix;
        }

        public static string GetTargetPath()
        {
            return NetworkResourcePath.ServerCreator_targetPath;
        }

        public static string GetServerDir(string serverName)
        {
            var serverDir = NetworkResourcePath.ServerCreator_targetPath + "/" + serverName;

            if (!Directory.Exists(serverDir))
            {
                Directory.CreateDirectory(serverDir);
            }

            return serverDir;
        }
    }
}
#endif