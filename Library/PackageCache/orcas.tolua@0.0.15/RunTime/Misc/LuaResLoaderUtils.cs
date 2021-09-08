using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LuaInterface
{
    public class LuaResLoaderUtils : LuaFileUtils
    {
        public static Func<byte[], byte[]> OnLoadBundle;
        public static string LuaInitFilesName = ""; //LuaInitFiles
        private readonly Dictionary<string, byte[]> _luaFiles = new Dictionary<string, byte[]>(128);

        public LuaResLoaderUtils()
        {
            instance = this;
            beZip = false;
        }

        private string[] GetLuaFileLists()
        {
            var fileListPath = Application.persistentDataPath + "/FileList.csv";
            if (File.Exists(fileListPath) == false)
                return null;
            var content = File.ReadAllText(fileListPath);
            Debug.Log(content);
            var lines = content.Split('\n', '\r');
            var luaFiles = new List<string>(128);
            for (int i = 3; i < lines.Length; i++)
            {
                var line = lines[i].Split(',');
                if (line.Length >= 2 && line[0].EndsWith(".luabundle", StringComparison.Ordinal))
                    luaFiles.Add(line[1]);
            }
            return luaFiles.ToArray();
        }

        private void LoadLuaBundle()
        {
            string[] fileNames = GetLuaFileLists();
            if (fileNames == null || fileNames.Length == 0)
            {
                Debug.LogWarning("file list none");
                return;
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var rootPath = Application.persistentDataPath + "/";
            foreach (var fileName in fileNames)
            {
                var bundle = AssetBundle.LoadFromFile(rootPath + fileName);
                if (bundle == null)
                    continue;
                var luaFileNames = bundle.GetAllAssetNames();
                if (luaFileNames == null || luaFileNames.Length == 0)
                    continue;
                foreach (var luaFileName in luaFileNames)
                {
                    var asset = bundle.LoadAsset<TextAsset>(luaFileName);
                    if (asset == null)
                        continue;
                    var luaName = GetLuaName(luaFileName);
                    if (_luaFiles.ContainsKey(luaName))
                    {
                        Debug.LogError(luaName + " have been added");
                        _luaFiles.Remove(luaName);
                    }
                    else
                    {
                        //Debug.Log("add " + luaName);
                    }
                    if (OnLoadBundle != null)
                        _luaFiles.Add(luaName, OnLoadBundle(asset.bytes));
                    else
                        _luaFiles.Add(luaName, asset.bytes);
                }
                bundle.Unload(true);
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
#if UNITY_EDITOR
            Debug.LogError("init luabundle time " + elapsedMs + " ms/" + fileNames.Length);
#else
            Debug.Log("init luabundle time " + elapsedMs + " ms/" + fileNames.Length);
#endif
        }

        public override byte[] ReadFile(string fileName)
        {
#if UNITY_EDITOR && !ASSETSBUNDLE_TEST
            var bytes = base.ReadFile(fileName);
            if (bytes == null)
                bytes = ReadResourceFile(fileName);
            if (bytes == null)
                bytes = ReadCacheFile(fileName);
#else
            var bytes = ReadCacheFile(fileName);
            if (bytes == null)
                bytes = ReadResourceFile(fileName);
            if (bytes == null)
                bytes = base.ReadFile(fileName);
#endif
            return bytes;
        }

        private string GetLuaName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName).ToLower().Replace(".lua", "").Replace(".bytes", "").Replace(".txt", "");
        }

        byte[] ReadCacheFile(string fileName)
        {
            var luaName = GetLuaName(fileName);
            if (_luaFiles.TryGetValue(luaName, out var bytes))
                return bytes;
            return null;
        }

        byte[] ReadResourceFile(string fileName)
        {
            if (!fileName.EndsWith(".lua"))
            {
                fileName += ".lua";
            }

            byte[] buffer = null;
            string path = "Lua/" + fileName;
            TextAsset text = Resources.Load(path, typeof(TextAsset)) as TextAsset;

            if (text != null)
            {
                buffer = text.bytes;
                Resources.UnloadAsset(text);
            }

            return buffer;
        }

        public override void LoadLuaFiles()
        {
            base.LoadLuaFiles();
            LoadLuaBundle();
        }

        public override void OnInitFinish(LuaState luaState)
        {
            base.OnInitFinish(luaState);
            if (string.IsNullOrWhiteSpace(LuaInitFilesName))
                return;
            LuaTable fileTable = luaState.DoFile<LuaTable>(LuaInitFilesName);
            for (int i = 1; i <= fileTable.Length; i++)
            {
                luaState.DoFile(fileTable[i].ToString());
            }
        }

        public override void Dispose()
        {
            OnLoadBundle = null;
            _luaFiles?.Clear();
            base.Dispose();
        }
    }
}
