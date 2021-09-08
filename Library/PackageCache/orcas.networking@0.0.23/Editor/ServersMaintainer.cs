#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Experimental;

namespace Orcas.Networking
{

    public class ServersMaintainer : MonoBehaviour
    {
        [SerializeField]
        private static ServersMaintainData _maintainData;

        public static string GetMaintainDataPath()
        {

            return NetworkResourcePath.ServerMaintainDataPath;
        }

        [ExecuteInEditMode, InitializeOnLoadMethod]
        static void Main(string[] args)
        {
            //Debug.Log("Call Maintainer Main Method!");

            if (CheckMaintainData())
                CheckData();
        }

        private static void MaintainUpdate()
        {
                CheckData();
                EditorApplication.delayCall -= MaintainUpdate;
        }




        private static bool CheckMaintainData()
        {
            int index = NetworkResourcePath.ServerMaintainDataPath.LastIndexOf("/");
            var maintainDir = NetworkResourcePath.ServerMaintainDataPath.Substring(0, index);
            CheckDirectory(maintainDir);
            var fileExists = File.Exists(NetworkResourcePath.ServerMaintainDataPath);
            _maintainData = AssetDatabase.LoadAssetAtPath<ServersMaintainData>(NetworkResourcePath.ServerMaintainDataPath);
            if (_maintainData != null)
            {
                var dataCount = _maintainData.GetBriefs().Count;

                Debug.Log("maintainData exists!");
                Debug.Log("data Count: " + dataCount);
            }
            else
            {
                if (fileExists)
                {

                    Debug.Log("is importing  ");
                    EditorApplication.delayCall += MaintainUpdate;
                    return false;
                }
                Debug.Log("maintainData not exists!");

                EditorApplication.delayCall += CreateAndCheck;
                return false;
            }

            return true;
        }

        static void CreateAndCheck()
        {
            CreateNewAsset();
            CheckData();
            EditorApplication.delayCall -= CreateAndCheck;
        }

        static void CreateNewAsset()
        {
            _maintainData = ScriptableObject.CreateInstance<ServersMaintainData>();
            _maintainData.name = "ServersMaintainData";
            ReloadData();

            EditorUtility.SetDirty(_maintainData);
            AssetDatabase.CreateAsset(_maintainData, NetworkResourcePath.ServerMaintainDataPath);
            //_maintainData.name = "ServersMaintainData";     //需要起相同的名字，否则会丢失

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //ReloadData();
            var dataCount = _maintainData.GetBriefs().Count;
            Debug.Log("data Count: " + dataCount);
        }


        public static void DeleteServer(string serverName)
        {
            _maintainData.DeleteServerBrief(serverName);
            DeleteServerDir(serverName);
        }

        public static void AddServer(ServerConfig serverConfig)
        {
            if (_maintainData.ContainsServer(serverConfig.GetServerName()))
            {
                DeleteServer(serverConfig.GetServerName());
            }

            ServerCreator.Create(serverConfig);
            var apiBriefList = APICreator.CreateAPIScripts(serverConfig);

            _maintainData.AddServerBrief(serverConfig.GetServerName(), apiBriefList);
            AssetDatabase.SaveAssets();
        }

        private static void ReloadData()
        {
            if (!CheckDirectory(NetworkResourcePath.ServerConfigDir)) return;

            DirectoryInfo directory = new DirectoryInfo(NetworkResourcePath.ServerConfigDir);

            FileInfo[] files = directory.GetFiles("*.asset", SearchOption.AllDirectories);
            Debug.Log("ReloadData filesCount: " + files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i].Name;
                ServerConfig config = AssetDatabase.LoadAssetAtPath<ServerConfig>(NetworkResourcePath.ServerConfigDir + "/" + fileName);
                var apiList = config.GetApiList();
                var apiBriefList = new List<ServersMaintainData.APIBrief>();
                var serverName = config.GetServerName();
                for (int j = 0; j < apiList.Count; j++)
                {
                    var apiPath = apiList[j].GetPath();
                    var apiName = APICreator.CreateClassName(apiPath, serverName);
                    var apiFormat = apiList[j].GetFormat();
                    apiBriefList.Add(new ServersMaintainData.APIBrief(apiName, apiPath, apiFormat));
                }

                _maintainData.AddServerBrief(serverName, apiBriefList);
                //Debug.Log("maintainData addBrief: " + serverName);
            }
            //AssetDatabase.SaveAssets();
            //  AssetDatabase.Refresh();
        }

        private static void CheckData()
        {
            Debug.Log("Check Data");
            var path = ServerCreator.GetTargetPath();
            CheckDirectory(path);

            var directory = new DirectoryInfo(path);
            var serverFolders = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);
            /*

            for (int i = 0; i < _maintainData.GetBriefs().Count; i++)
            {
                var brief = _maintainData.GetBriefs()[i];
                Debug.Log("AAA  apiBrief Count: " + brief.APIBriefList.Count);
            }
            */
            for (int i = 0; i < serverFolders.Length; i++)
            {
                var serverFolder = serverFolders[i];
                var serverFolderName = serverFolder.Name;   //由于是目录文件后面结尾应该没有格式
                if (!_maintainData.ContainsServer(serverFolderName)) continue;

                // Debug.Log("Server Folder Name: " + serverFolderName);

                //检查server的.lua和.cs文件是否存在
                var server_lua = serverFolder.GetFiles(serverFolderName + ".lua", SearchOption.TopDirectoryOnly);
                var server_cs = serverFolder.GetFiles(serverFolderName + ".cs", SearchOption.TopDirectoryOnly);

                if (server_lua.Length == 0)
                {
                    var serverConfig = AssetDatabase.LoadAssetAtPath<ServerConfig>(NetworkResourcePath.ServerConfigDir + "/" + serverFolderName + ".asset");
                    ServerCreator.CreateServer_Lua(serverConfig);
                }
                if (server_cs.Length == 0)
                {
                    var serverConfig = AssetDatabase.LoadAssetAtPath<ServerConfig>(NetworkResourcePath.ServerConfigDir + "/" + serverFolderName + ".asset");
                    ServerCreator.CreateServer_CS(serverConfig);
                }

                //检查server文件夹中的目录API是否存在
                var apiDirs = serverFolder.GetDirectories("API", SearchOption.TopDirectoryOnly);
                if (apiDirs.Length == 0)
                {
                    Debug.Log("Can't find API folder in " + serverFolderName + " folder");
                    serverFolder.CreateSubdirectory("API");
                    Debug.Log("Create API folder in " + serverFolderName + " folder");
                    var serverConfig = AssetDatabase.LoadAssetAtPath<ServerConfig>(NetworkResourcePath.ServerConfigDir + "/" + serverFolderName + ".asset");
                    Debug.Log("Load serverconfig for " + serverFolderName);
                    APICreator.CreateAPIScripts(serverConfig);
                    Debug.Log("Create APIs in " + serverFolderName + "'s API folder");
                    //需要设置API文件的创建路径
                }
                else
                {
                    //检查API中的文件
                    var apiDir = apiDirs[0];
                    var apiCSFiles = apiDir.GetFiles("*.cs", SearchOption.TopDirectoryOnly);
                    var apiLuaFiles = apiDir.GetFiles("*.lua", SearchOption.TopDirectoryOnly);
                    List<ServersMaintainData.APIBrief> requiredApiBriefs = new List<ServersMaintainData.APIBrief>();
                    requiredApiBriefs.AddRange(_maintainData.GetServerApiBriefList(serverFolderName));
                    var missing_Luas = new List<string>();
                    var missing_CSs = new List<string>();

                    foreach (var brief in requiredApiBriefs)
                    {
                        if (brief.Format != APIInfo.Format.CSharp)
                        {
                            bool missing = true;
                            foreach (var luaFile in apiLuaFiles)
                            {

                                if (brief.APIName + ".lua" == luaFile.Name)
                                {
                                    missing = false;
                                    break;
                                }
                            }

                            if (missing)
                            {
                                missing_Luas.Add(brief.Path);
                            }
                        }

                        if (brief.Format != APIInfo.Format.Lua)
                        {
                            bool missing = true;
                            foreach (var csFile in apiCSFiles)
                            {
                                if (brief.APIName + ".cs" == csFile.Name)
                                {
                                    missing = false;
                                    break;
                                }
                            }

                            if (missing)
                            {
                                missing_CSs.Add(brief.Path);
                            }
                        }
                    }

                    if (missing_CSs.Count > 0)
                    {
                        Debug.Log("missing_CSharp files.Count: " + missing_CSs.Count);

                        foreach (var CSFile in apiCSFiles)
                        {
                            Debug.Log("CSFile Name: " + CSFile.Name);
                        }
                    }

                    if (missing_Luas.Count > 0)
                    {
                        Debug.Log("missing_Lua files.Count: " + missing_Luas.Count);

                        foreach (var luaFile in apiLuaFiles)
                        {

                            Debug.Log("LuaFile Name: " + luaFile.Name);
                        }
                    }

                    if (missing_CSs.Count + missing_Luas.Count > 0)
                    {
                        //Debug.Log("file missing");
                        var serverConfig = AssetDatabase.LoadAssetAtPath<ServerConfig>(NetworkResourcePath.ServerConfigDir + "/" + serverFolderName + ".asset");
                        List<APIInfo> apiInfoList = serverConfig.GetApiList();

                        if (missing_CSs.Count > 0)
                        {
                            foreach (var missing_CS in missing_CSs)
                            {
                                foreach (var apiInfo in apiInfoList)
                                {
                                    if (apiInfo.GetFormat() != APIInfo.Format.Lua && apiInfo.GetPath() == missing_CS)
                                    {
                                        APICreator.CreateCSAPIScript(apiInfo, serverFolderName);
                                        break;
                                    }
                                }
                            }
                        }

                        if (missing_Luas.Count > 0)
                        {
                            foreach (var missing_Lua in missing_Luas)
                            {
                                foreach (var apiInfo in apiInfoList)
                                {
                                    if (apiInfo.GetFormat() != APIInfo.Format.CSharp && apiInfo.GetPath() == missing_Lua)
                                    {
                                        APICreator.CreateLuaAPIScript(apiInfo, serverFolderName);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /*
            for (int i = 0; i < _maintainData.GetBriefs().Count; i++)
            {
                var brief = _maintainData.GetBriefs()[i];
                Debug.Log("BBB apiBrief Count: " + brief.APIBriefList.Count);
            }
            */

            var requiredFolderNames = _maintainData.GetServerNameList();
            foreach (var serverFolder in serverFolders)
            {
                if (requiredFolderNames.Contains(serverFolder.Name))
                    requiredFolderNames.Remove(serverFolder.Name);
            }

            Debug.Log("Required folders count: " + requiredFolderNames.Count);
            if (!CheckDirectory(NetworkResourcePath.ServerConfigDir))
            {
                Debug.Log("Can not find config directory: " + NetworkResourcePath.ServerConfigDir);

            }
            else
            {
                foreach (var requiredFolderName in requiredFolderNames)
                {
                    var serverConfig = AssetDatabase.LoadAssetAtPath<ServerConfig>(NetworkResourcePath.ServerConfigDir + "/" + requiredFolderName + ".asset");
                    if (serverConfig == null)
                    {
                        Debug.Log("can not fing config name: " + NetworkResourcePath.ServerConfigDir + "/" + requiredFolderName + ".asset");
                        Debug.Log("Error!serverconfig is null");
                    }
                    else
                    {
                        ServerCreator.Create(serverConfig);
                        APICreator.CreateAPIScripts(serverConfig);
                    }
                }
            }
            //Debug.Log("MaintainData Count before over: " + _maintainData.GetBriefs().Count);

            //_maintainData = AssetDatabase.LoadAssetAtPath<ServersMaintainData>(_maintainDataPath);
            // Debug.Log("MaintainData Count load agian before over: " + _maintainData.GetBriefs().Count);
            /*
            for (int i = 0; i < _maintainData.GetBriefs().Count; i++)
            {
                var brief = _maintainData.GetBriefs()[i];
                Debug.Log("CCC apiBrief Count: " + brief.APIBriefList.Count);
            }
            */
            Debug.Log("Check Data Over");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void DeleteServerDir(string serverName)
        {
            var path = ServerCreator.GetTargetPath() + "/" + serverName;
            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                directory.Delete(true);
                Debug.Log("Delete Server Folder");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("Can't find Server Folder");
            }
        }

        public static bool CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return false;
            }
            return true;
        }
    }
}
#endif