using System;
using System.Globalization;
using System.Text;
using LuaInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orcas.Core;
using UnityEngine;

namespace Orcas.Data
{
    public class StorageManager : IManager
    {
        private int _playerId;
        private Storage _root;
        private Storage _version;
        
        /// <summary>
        /// 设置玩家的ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Storage SetUserId(int id)
        {
            _playerId = id;
            _root = new Storage(null, id.ToString());
            _version = GetOrCreateStorage("version");
            return _root;
        }
        
        /// <summary>
        /// 获取对应路径上的存储器
        /// </summary>
        /// <param name="root"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static Storage GetOrCreateStorage(Storage root, string[] names)
        {
            return root.GetChild(names);
        }

        /// <summary>
        /// 获取对应路径上的存储器
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Storage GetOrCreateStorage(Storage root, string path)
        {
            return root.GetChild(path);
        }

        /// <summary>
        /// 获取对应路径上的存储器
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Storage GetOrCreateStorage(string path)
        {
            return _root.GetChild(path);
        }
        
        /// <summary>
        /// 获取对应路径上的存储器
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public Storage GetOrCreateStorage(params string[] names)
        {
            return _root.GetChild(names);
        }

        /// <summary>
        /// 保存浮点数到指定路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void SetFloat(string path, float value)
        {
            _root.GetChild(path).FloatValue = value;
        }
        
        /// <summary>
        /// 保存浮点数到指定路径
        /// </summary>
        /// <param name="names"></param>
        /// <param name="value"></param>
        public void SetFloat(string[] names, float value)
        {
            _root.GetChild(names).FloatValue = value;
        }
        
        /// <summary>
        /// 保存整数到指定路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void SetInt(string path, int value)
        {
            _root.GetChild(path).IntValue = value;
        }
        
        /// <summary>
        /// 保存整数到指定路径
        /// </summary>
        /// <param name="names"></param>
        /// <param name="value"></param>
        public void SetInt(string[] names, int value)
        {
            _root.GetChild(names).IntValue = value;
        }
        
        /// <summary>
        /// 保存字符串到指定路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void SetString(string path, string value)
        {
            _root.GetChild(path).StringValue = value;
        }
        
        /// <summary>
        /// 保存字符串到指定路径
        /// </summary>
        /// <param name="names"></param>
        /// <param name="value"></param>
        public void SetString(string[] names, string value)
        {
            _root.GetChild(names).StringValue = value;
        }
        

        /// <summary>
        /// 重写本地的版本信息
        /// 在上传文件之前务必进行一次版本重设
        /// </summary>
        public void ReSetLocalVersion()
        {
            _version.StringValue = SystemInfo.deviceUniqueIdentifier + "," + DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// 获取本地存储的json数据
        /// </summary>
        /// <returns></returns>
        public string GetJsonData()
        {
            return _root.GetJsonData();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            PlayerPrefs.Save();
        }

        private static void LoadFromJson(Storage root, string path, JToken jToken)
        {
            var storage = GetOrCreateStorage(root, path);
            var jtokenT = jToken["t"];
            if (jtokenT == null) return;
            var type = (StorageType) (byte) jtokenT;
            switch (type)
            {
                case StorageType.Int:
                    storage.IntValue = (int)jToken["v"];
                    break;
                case StorageType.Float:
                    storage.FloatValue = (float)jToken["v"];
                    break;
                case StorageType.None:
                    break;
                case StorageType.String:
                    storage.StringValue = (string)jToken["v"];
                    break;
                case StorageType.Node:
                    var first = jToken["v"].First;
                    var last = jToken["v"].Last;
                    while(true)
                    {
                        var property = first.ToObject<JProperty>();
                        LoadFromJson(root, $"{path}.{property.Name}", property.Value);
                        if (first == last) break;
                        first = first.Next;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 从json数据中加载存储文件
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static Storage LoadFromJsonData(int userId, string jsonData)
        {
            var root = new Storage(null, userId.ToString());
            var jToken = JsonConvert.DeserializeObject<JToken>(jsonData);
            LoadFromJson(root, string.Empty, jToken);
            return root;
        }

        /// <summary>
        /// 检查是否与线上版本数据冲突
        /// 需要确认完成登录和本地数据库初始化操作
        /// </summary>
        /// <param name="webStorage"></param>
        /// <returns></returns>
        public StorageVersionCheckResult CheckStorageVersionCrash(Storage webStorage)
        {
            if (string.Equals(webStorage.Name, _root.Name, StringComparison.Ordinal) == false)
            {
                Debug.LogError("[Storage] 线上数据文件错误!user id不一致!以本地版本为准处理冲突...");
                return StorageVersionCheckResult.ExceptionSuggestLocal;
            }

            var webVersion = webStorage.GetChild("version");
            var webVersionCode = webVersion.StringValue;
            var localVersionCode = _version.StringValue;
            if (string.IsNullOrEmpty(localVersionCode)) return StorageVersionCheckResult.Crash;
            if (string.IsNullOrEmpty(webVersionCode))
            {
                Debug.LogError("[Storage] 线上数据文件错误!找不到version代码!以本地版本为准处理冲突...");
                return StorageVersionCheckResult.ExceptionSuggestLocal;
            }

            var webVersions = webVersionCode.Split(',');
            var localVersions = localVersionCode.Split(',');
            if (webVersions.Length != 2)
            {
                Debug.LogError("[Storage] 未知原因导致存储器版本号异常!以本地版本为准处理冲突...");
                return StorageVersionCheckResult.ExceptionSuggestLocal;
            }

            if (localVersions.Length != 2)
            {
                Debug.LogError("[Storage] 未知原因导致存储器版本号异常!以服务器版本为准处理冲突...");
                return StorageVersionCheckResult.ExceptionSuggestWeb;
            }

            if (string.Equals(webVersions[0], localVersions[0], StringComparison.Ordinal))
            {
                try
                {
                    var webTicks = long.Parse(webVersions[1], NumberStyles.None);
                    var localTicks = long.Parse(localVersions[1], NumberStyles.None);
                    return webTicks <= localTicks ? StorageVersionCheckResult.Access : StorageVersionCheckResult.Crash;
                }
                catch
                {
                    return StorageVersionCheckResult.Exception;
                }
            }
            else
            {
                return StorageVersionCheckResult.Crash;
            }
        }
        
        /// <summary>
        /// 使用云端数据替换本地数
        /// 替换后需要重新加载一次游戏
        /// 重新绑定监听事件
        /// </summary>
        /// <param name="webStorage"></param>
        public void ResetStorage(Storage webStorage)
        {
            _root.DeleteFromLocal();
            _root = webStorage;
            _root.SaveToLocal();
            _version = GetOrCreateStorage("version");
            Save();
        }
        
        [NoToLua]
        public void Init()
        {
        }
        [NoToLua]
        public void Update(uint currentFrameCount)
        {
        }
        [NoToLua]
        public void OnPause()
        {
        }
        [NoToLua]
        public void OnResume()
        {
        }
        [NoToLua]
        public void OnDestroy()
        {
        }
    }
}