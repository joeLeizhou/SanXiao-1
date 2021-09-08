#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental;

namespace Orcas.Networking
{
    public class ProtosMaintainer : MonoBehaviour
    {
        [SerializeField]
        private static ProtosMaintainData _maintainData;


        [ExecuteInEditMode, InitializeOnLoadMethod]
        static void Main(string[] args)
        {
            //Debug.Log("Call ProtosMaintainer Main Method!"); 

            if (CheckMaintainData())
                BeginCheck();
        }

        private static void MaintainUpdate()
        {
                BeginCheck();
                EditorApplication.delayCall -= MaintainUpdate;
        }

        private static void BeginCheck()
        {
            CheckProtos();
            CheckLuaProtos();
        }

        private static bool CheckMaintainData()
        {
            int index = NetworkResourcePath.ProtoMaintainDataPath.LastIndexOf("/");
            var maintainDir = NetworkResourcePath.ProtoMaintainDataPath.Substring(0, index);
            CheckDirectory(maintainDir);
            var fileExists = File.Exists(NetworkResourcePath.ProtoMaintainDataPath);
            _maintainData = AssetDatabase.LoadAssetAtPath<ProtosMaintainData>(NetworkResourcePath.ProtoMaintainDataPath);
            if (_maintainData != null)
            {
                var protoCount = _maintainData.GetProtoList().Count + _maintainData.GetLuaProtoList().Count;

                Debug.Log("ProtosMaintainData exists!");
                Debug.Log("protoCount: " + protoCount);
            }
            else
            {
                if (fileExists)
                {
                    Debug.Log("is importing  ");
                    EditorApplication.delayCall += MaintainUpdate;
                    return false;
                }
                Debug.Log("ProtosMaintainData not exists!");
             

                EditorApplication.delayCall += CreateAndCheck;
                return false;
            }

            return true;
        }


        static void CreateAndCheck()
        {
            CreateNewAsset();
            BeginCheck();
            EditorApplication.delayCall -= CreateAndCheck;
        }

        static void CreateNewAsset()
        {
            _maintainData = ScriptableObject.CreateInstance<ProtosMaintainData>();
            _maintainData.name = "ProtosMaintainData";
            ReloadData();

            EditorUtility.SetDirty(_maintainData);
            AssetDatabase.CreateAsset(_maintainData, NetworkResourcePath.ProtoMaintainDataPath);
            // _maintainData.name = "ProtosMaintainData";     //需要起相同的名字，否则会丢失

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            var protoCount = _maintainData.GetProtoList().Count + _maintainData.GetLuaProtoList().Count;
            Debug.Log("protoCount: " + protoCount);
        }

        public static void DeleteProto(string protoName)
        {
            _maintainData.DeleteProto(protoName);
            var path = NetworkResourcePath.CSProtoCreator_targetPath;
            DirectoryInfo directory = new DirectoryInfo(path);
            var files = directory.GetFiles(protoName + "*");
            foreach (var file in files)
            {
                file.Delete();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void DeleteLuaProto(string protoName)
        {
            _maintainData.DeleteLuaProto(protoName);
            var path = NetworkResourcePath.LuaProtoCreator_targetPath;
            DirectoryInfo directory = new DirectoryInfo(path);
            var files = directory.GetFiles(protoName + "*");
            foreach (var file in files)
            {
                file.Delete();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void AddProto(ProtoConfig config)
        {
            _maintainData.AddProto(config.GetProtoName());
            ProtoCreator.Create(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void AddProto(LuaProtoConfig config)
        {
            _maintainData.AddLuaProto(config.GetProtoName());
            LuaProtoCreator.Create(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ReloadData()
        {
            ReloadCSProto();
            ReloadLuaProto();
        }

        private static void ReloadCSProto()
        {
            var configPath = NetworkResourcePath.CSProtoConfigDir;
            if (!CheckDirectory(configPath)) return;
            DirectoryInfo directory = new DirectoryInfo(configPath);
            FileInfo[] files = directory.GetFiles("*.asset", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = file.Name;
                var strs = fileName.Split('.');
                var nameWithoutExt = strs[0];
                //Debug.Log("nameWithoutExt: " + nameWithoutExt);
                if (nameWithoutExt != "NewProto")
                {
                    _maintainData.AddProto(nameWithoutExt);
                }
            }
        }

        private static void ReloadLuaProto()
        {
            var configPath = NetworkResourcePath.LuaProtoConfigDir;
            if (!CheckDirectory(configPath)) return;
            DirectoryInfo directory = new DirectoryInfo(configPath);
            FileInfo[] files = directory.GetFiles("*.asset", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = file.Name;
                var strs = fileName.Split('.');
                var nameWithoutExt = strs[0];
                //Debug.Log("nameWithoutExt: " + nameWithoutExt);
                if (nameWithoutExt != "NewProto")
                {
                    _maintainData.AddLuaProto(nameWithoutExt);
                }
            }
        }

        private static void CheckProtos()
        {
            var path = NetworkResourcePath.CSProtoCreator_targetPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var directory = new DirectoryInfo(path);
            var currentProtos = directory.GetFiles("*.cs", SearchOption.AllDirectories);
            var requiredProtoList = _maintainData.GetProtoList();

            for (int i = 0; i < currentProtos.Length; i++)
            {
                var currentProto = currentProtos[i];
                foreach (var requiredProto in requiredProtoList)
                {
                    if (requiredProto + ".cs" == currentProto.Name)
                    {
                        requiredProtoList.Remove(requiredProto);
                        break;
                    }
                }
            }

            CheckDirectory(NetworkResourcePath.CSProtoConfigDir);
            foreach (var requiredProto in requiredProtoList)
            {
                var protoConfig = AssetDatabase.LoadAssetAtPath<ProtoConfig>(NetworkResourcePath.CSProtoConfigDir + "/" + requiredProto + ".asset");
                if (protoConfig != null)
                    ProtoCreator.Create(protoConfig);
            }

            AssetDatabase.SaveAssets();


            AssetDatabase.Refresh();
        }

        private static void CheckLuaProtos()
        {
            var path = NetworkResourcePath.LuaProtoCreator_targetPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var directory = new DirectoryInfo(path);
            var currentProtos = directory.GetFiles("*.lua", SearchOption.AllDirectories);
            var requiredProtoList = _maintainData.GetLuaProtoList();

            for (int i = 0; i < currentProtos.Length; i++)
            {
                var currentProto = currentProtos[i];
                foreach (var requiredProto in requiredProtoList)
                {
                    if (requiredProto + ".lua" == currentProto.Name)
                    {
                        requiredProtoList.Remove(requiredProto);
                        break;
                    }
                }
            }

            CheckDirectory(NetworkResourcePath.LuaProtoConfigDir);
            foreach (var requiredProto in requiredProtoList)
            {
                var luaProtoConfig = AssetDatabase.LoadAssetAtPath<LuaProtoConfig>(NetworkResourcePath.LuaProtoConfigDir + "/" + requiredProto + ".asset");
                if (luaProtoConfig != null)
                    LuaProtoCreator.Create(luaProtoConfig);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

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