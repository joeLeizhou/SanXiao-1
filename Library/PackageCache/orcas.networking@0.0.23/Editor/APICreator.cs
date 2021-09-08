#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Orcas.Networking
{
    public class APICreator
    {
        public static List<ServersMaintainData.APIBrief> CreateAPIScripts(ServerConfig config)
        {
            var apiInfoLists = config.GetApiList();
            var serverName = config.GetServerName();
            var apiBriefLists = new List<ServersMaintainData.APIBrief>();
            for (int i = 0; i < apiInfoLists.Count; i++)
            {
                var apiInfo = apiInfoLists[i];
                if (apiInfo.GetPath() == "")
                    continue;

                var format = apiInfo.GetFormat();
                if (format != APIInfo.Format.Lua)
                {
                    CreateCSAPIScript(apiInfo, serverName); 
                }
                else if (format != APIInfo.Format.CSharp)
                {
                    CreateLuaAPIScript(apiInfo, serverName);
                }

                var apiName = CreateClassName(apiInfo.GetPath(), serverName);
                apiBriefLists.Add(new ServersMaintainData.APIBrief(apiName, apiInfo.GetPath(), format));    
            }
            return apiBriefLists;
        }

        public static string CreateCSAPIScript(APIInfo apiInfo, string serverName)
        {
            var serverDir = ServerCreator.GetTargetPath();
            var targetPath = serverDir + "/" + serverName + "/" + "API";
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var className = CreateClassName(apiInfo.GetPath(), serverName);
            var filePath = targetPath + "/" + className + ".cs";

            if (Directory.Exists(filePath))
            {
                AssetDatabase.DeleteAsset(filePath);
            }

            (var arguments, var argStrs) = CreateCSCallAPIArgs(apiInfo.GetParameterList());
            // Debug.Log("parameter list count: " + parameterList.Count);      

            using (StreamWriter outfile =
                     new StreamWriter(filePath))
            {
                outfile.WriteLine("using UnityEngine;");
                outfile.WriteLine("using System.Collections;");
                outfile.WriteLine("using System.Collections.Generic;");
                outfile.WriteLine("using Orcas.Networking;");
                outfile.WriteLine("");

                outfile.WriteLine(" public class " + className);
                outfile.WriteLine("{");
                outfile.WriteLine(" " + "   " + "private ServerConfig _config;");
                outfile.WriteLine(" " + "   " + "private int _apiIndex;");
                outfile.WriteLine(" ");

                outfile.WriteLine(" " + "   public " + className + "(ServerConfig config, int apiIndex)");
                outfile.WriteLine(" " + "   {");
                outfile.WriteLine(" " + "   " + "   _config = config;");
                outfile.WriteLine(" " + "   " + "   _apiIndex = apiIndex;");
                outfile.WriteLine(" " + "   }");

                outfile.WriteLine(" ");
                outfile.WriteLine(" " + "   " + "public int CallAPI(" + arguments + ") ");
                outfile.WriteLine(" " + "   {");
                outfile.WriteLine(" " + "   " + "   " + "var apiInfo = _config.GetApiList()[_apiIndex];");
                outfile.WriteLine(" " + "   " + "   " + "var path = apiInfo.GetPath();");
                outfile.WriteLine(" " + "   " + "   " + "var url = _config.GetDefaultUrlInfo().GetUrl();");

                for (int i = 0; i < argStrs.Count; i++)
                {
                    outfile.WriteLine(" " + "   " + "   " + argStrs[i]);

                }
                /*
                if (parameterNameList.Count > 0)
                {
                    outfile.WriteLine(" " + "   " + "   " + "string args = \"" + parameterNameList[0] + "=\" + " + parameterNameList[0] + ";");
                } 
                else
                {
                    outfile.WriteLine(" " + "   " + "   " + "string args = \"\";");
                }
                if (parameterNameList.Count > 1)
                {
                    for (int i = 1; i < parameterNameList.Count; i++) {
                        outfile.WriteLine(" " + "   " + "   " + "args += \"&" + parameterNameList[i] + "=\" + " + parameterNameList[i] + ";");
                    }
                }*/

                outfile.WriteLine(" " + "   " + "");
                outfile.WriteLine(" " + "   " + "   " + "int requestID = " + serverName + ".Instance." + GetUnityWebRequestFuncStr_CS(apiInfo.GetMethod()) + ";");
                outfile.WriteLine(" " + "   " + "   " + "return requestID;");
                outfile.WriteLine(" " + "   }");
                outfile.WriteLine(" ");

                outfile.WriteLine(" " + "   " + "public void CallBack(IResponseData resData, int requestID)");
                outfile.WriteLine(" " + "   " + "{");
                outfile.WriteLine(" ");
                outfile.WriteLine(" " + "   }");
                outfile.WriteLine("}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return className;
        }

        public static string CreateLuaAPIScript(APIInfo apiInfo, string serverName)
        {
            var serverDir = ServerCreator.GetTargetPath();
            var targetPath = serverDir + "/" + serverName + "/" + "API";
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var className = CreateClassName(apiInfo.GetPath(), serverName);
            var filePath = targetPath + "/" + className + ".lua";
            if (Directory.Exists(filePath))
            {
                AssetDatabase.DeleteAsset(filePath);
            }

            var arguments = GetParameterNameList(apiInfo.GetParameterList());

            var definition_Strs = CreateLuaAPI_Definitions(className);
            var funcStrs_new = CreateLuaAPIFunc_new(className);
            var funcStrs_CallAPI = CreateLuaAPIFunc_CallAPI(className, arguments, apiInfo.GetMethod());
            var funcStrs_CallBack = CreateLuaAPIFunc_CallBack(className);
            var apiOrders = CreateLuaAPI_Orders(className);

            var funcStrsList = new List<List<string>>();

            funcStrsList.Add(definition_Strs);
            funcStrsList.Add(funcStrs_new);
            funcStrsList.Add(funcStrs_CallAPI);
            funcStrsList.Add(funcStrs_CallBack);
            funcStrsList.Add(apiOrders);

            WriteAPIFile(className, filePath, funcStrsList);
            return className;
        }

        private static void WriteAPIFile(string className, string filePath, List<List<string>> funcStrsList)
        {
            using (StreamWriter outfile =
                    new StreamWriter(filePath))
            {
                for (int i = 0; i < funcStrsList.Count; i++)
                {
                    var funcStrs = (funcStrsList[i]);
                    for (int j = 0; j < funcStrs.Count; j++)
                    {
                        outfile.WriteLine(funcStrs[j]);
                    }

                    outfile.WriteLine(" ");

                }

                outfile.WriteLine(" ");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string CreateClassName(string apiPath, string serverName)
        {
            string apiName = "";
            apiName += serverName;
            for (int i = 0; i < apiPath.Length; i++)
            {
                if (apiPath[i] == '/' || apiPath[i] == '.')
                {
                    apiName += "_";
                }
                else if (apiPath[i] == '-')
                {
                    i++;
                    if (i < apiPath.Length)
                    {
                        char c = apiPath[i];
                        if (c >= 97 && c <= 122)
                        {
                            c = (char)(c - 32);
                            apiName += c;
                        }
                    }
                }
                else if (i == 0)
                {
                    char c = apiPath[i];
                    if (c >= 97 && c <= 122)
                    {
                        c = (char)(c - 32);
                        apiName += c;
                    }
                    else
                    {
                        apiName += c;
                    }
                }
                else
                {
                    apiName += apiPath[i];
                }
            }

            return apiName;
        }

        private static (string, List<string>) CreateCSCallAPIArgs(List<APIInfo.ParameterInfo> parameterList)
        {
            string arguments = "";
            var parameterNameList = new List<string>();
            for (int i = 0; i < parameterList.Count; i++)
            {
                var parameter = parameterList[i];
                string tmp = Enum.GetName(typeof(APIInfo.ValueType), parameter.type);
                string typeName = "";
                for (int j = 0; j < tmp.Length; j++)
                {
                    if (j == 0)
                    {
                        typeName += (char)(tmp[j] + 32);
                    }
                    else
                    {
                        typeName += tmp[j];
                    }
                }

                if (parameter.name == "" || parameter.name[0] == ' ')
                {
                    parameter.name = "para" + i.ToString();
                }

                parameterNameList.Add(parameter.name);

                arguments += typeName + " " + parameter.name;

                if (i != parameterList.Count - 1)
                {
                    arguments += ", ";
                }
            }

            List<string> argStrs = new List<string>();

            if (parameterNameList.Count > 0)
            {
                argStrs.Add("string args = \"" + parameterNameList[0] + "=\" + " + parameterNameList[0] + ";");
            }
            else
            {
                argStrs.Add("string args = \"\";");
            }

            if (parameterNameList.Count > 1)
            {
                for (int i = 1; i < parameterNameList.Count; i++)
                {
                    argStrs.Add("args += \"&" + parameterNameList[i] + "=\" + " + parameterNameList[i] + ";");
                }
            }

            return (arguments, argStrs);
        }

        private static List<string> GetParameterNameList(List<APIInfo.ParameterInfo> parameterList)
        {
            var parameterNameList = new List<string>();
            for (int i = 0; i < parameterList.Count; i++)
            {
                var parameter = parameterList[i];

                if (parameter.name == "" || parameter.name[0] == ' ')
                {
                    parameter.name = "para" + i.ToString();
                }

                parameterNameList.Add(parameter.name);
            }
            return parameterNameList;
        }

        private static List<string> CreateLuaAPI_Definitions(string className)
        {
            var definitions = new List<string>();
            var defineClassStr = "local " + className + " = {}";
            definitions.Add(defineClassStr);
            return definitions;
        }

        private static List<string> CreateLuaAPIFunc_new(string className)
        {
            var arguments = new List<string>();
            arguments.Add("config");
            arguments.Add("apiIndex");
            arguments.Add("Server");

            string funcHead = CreateLuaHelper.CreateLuaFunctionHead(className, "new", arguments);

            List<string> funcBody = new List<string>();
      
            for (int i = 0; i < arguments.Count; i++)
            {
                funcBody.Add("self." + arguments[i] + " = " + arguments[i]);
            }

           

            return CreateLuaHelper.CreateLuaFunction(funcHead, funcBody);
        }

        private static List<string> CreateLuaAPIFunc_CallAPI(string className, List<string> arguments, APIInfo.Method method)
        {
            string funcHead = CreateLuaHelper.CreateLuaFunctionHead(className, "CallAPI", arguments);

            List<string> funcBody = new List<string>();


            var setApiList = "local apiList = self.config.GetApiList(self.config)";

            var setApiInfo = "local apiInfo = apiList[self.apiIndex]";

            var setPath = "local path = apiInfo.GetPath(apiInfo)";

            var setUrlInfo = "local urlInfo = self.config.GetDefaultUrlInfo(self.config)";

            var setUrl = "local url = urlInfo.GetUrl(urlInfo);";

            var argStrs = CreateLuaCallAPIArgs(arguments);

            var setRequestID = "local requestID = self.Server." + GetUnityWebRequestFuncStr_Lua(method);

            var returnStatement = "return requestID";

            funcBody.Add(setApiList);
            funcBody.Add(setApiInfo);
            funcBody.Add(setPath);
            funcBody.Add(setUrlInfo);
            funcBody.Add(setUrl);
            funcBody.InsertRange(funcBody.Count, argStrs);
            funcBody.Add(" ");
            funcBody.Add(setRequestID);
            funcBody.Add(returnStatement);

            return CreateLuaHelper.CreateLuaFunction(funcHead, funcBody);
        }

        private static List<string> CreateLuaAPIFunc_CallBack(string className)
        {
            var arguments = new List<string>();
            arguments.Add("resData");
            arguments.Add("requestID");
            string funcHead = CreateLuaHelper.CreateLuaFunctionHead(className, "CallBack", arguments);
            List<string> funcBody = new List<string>();

            return CreateLuaHelper.CreateLuaFunction(funcHead, funcBody);
        }

        private static List<string> CreateLuaCallAPIArgs(List<string> arguments)
        {
            List<string> argStrs = new List<string>();

            if (arguments.Count > 0)
            {
                argStrs.Add("local args = \"" + arguments[0] + "=\".." + arguments[0]);
            }
            else
            {
                argStrs.Add("local args = \"\"");
            }

            if (arguments.Count > 1)
            {
                for (int i = 1; i < arguments.Count; i++)
                {
                    argStrs.Add("args = args..\"&" + arguments[i] + "=\".." + arguments[i]);
                }
            }

            return argStrs;
        }

        private static List<string> CreateLuaAPI_Orders(string className)
        {
            var returnStr = "return " + className;
            var orders = new List<string>();
            orders.Add(returnStr);

            return orders;
        }

        private static string GetUnityWebRequestFuncStr_Lua(APIInfo.Method method)
        {
            switch (method)
            {
                case APIInfo.Method.GET:
                    return "SendUnityWebRequest(self.Server, path, args, self.CallBack, true)";
                case APIInfo.Method.POST_FORM:
                    return "SendUnityFormWebRequest(self.Server, path, args, self.CallBack)";
                case APIInfo.Method.POST_URL:
                    return "SendUnityWebRequest(self.Server, path, args, self.CallBack, false)";
                case APIInfo.Method.POST_WWWForm:
                    return "SendUnityPostWWWFormWebRequest(self.Server, path, args, self.CallBack)";
                default:
                    return string.Empty;
            }
        }

        private static string GetUnityWebRequestFuncStr_CS(APIInfo.Method method)
        {
            switch (method)
            {
                case APIInfo.Method.GET:
                    return "SendUnityWebRequest(path, args, CallBack, true)";
                case APIInfo.Method.POST_FORM:
                    return "SendUnityFormWebRequest(path, args, CallBack)";
                case APIInfo.Method.POST_URL:
                    return "SendUnityWebRequest(path, args, CallBack, false)";
                case APIInfo.Method.POST_WWWForm:
                    return "SendUnityPostWWWFormWebRequest(path, args, CallBack)";
                default:
                    return string.Empty;
            }
        }
    }
}
#endif