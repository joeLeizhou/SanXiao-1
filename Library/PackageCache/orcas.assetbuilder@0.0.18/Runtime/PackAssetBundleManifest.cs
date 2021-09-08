using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Orcas.AssetBuilder
{
    [Serializable]
    [Preserve]
    public class PackAssetBundleInfo
    {
        [Preserve]
        public string Name;
        [Preserve]
        public string Hash128;
        [Preserve]
        public string[] Dependencies;
        [Preserve]
        [NonSerialized]
        public string[] AllDependencies;
        public PackAssetBundleInfo(string name, Hash128 hash128, string[] deps)
        {
            Name = name;
            Hash128 = hash128.ToString();
            Dependencies = deps;
        }
    }
    [Serializable]
    [Preserve]
    public class PackAssetBundleManifest
    {
        [Preserve]
        public PackAssetBundleInfo[] AssetBundleInfos;
        [Preserve]
        [NonSerialized]
        private Dictionary<string, PackAssetBundleInfo> _infos;
        public Dictionary<string, PackAssetBundleInfo> Infos { get { return _infos; } }
        public void Init()
        {
            if (AssetBundleInfos == null)
                AssetBundleInfos = new PackAssetBundleInfo[0];
            _infos = new Dictionary<string, PackAssetBundleInfo>(AssetBundleInfos.Length);
            for (int i = 0; i < AssetBundleInfos.Length; i++)
            {
                _infos[AssetBundleInfos[i].Name] = AssetBundleInfos[i];
            }
        }

        public void Save(string path, bool useInfo = false)
        {
            if (useInfo && _infos != null)
            {
                AssetBundleInfos = new PackAssetBundleInfo[_infos.Count];
                _infos.Values.CopyTo(AssetBundleInfos, 0);
            }
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
            Debug.Log("save pack Manifest " + path);
        }

        public static PackAssetBundleManifest Load(string path)
        {
            if (System.IO.File.Exists(path))
                return JsonUtility.FromJson<PackAssetBundleManifest>(System.IO.File.ReadAllText(path));
            return null;
        }

        public string[] GetAllAssetBundles()
        {
            string[] bundleNames = new string[AssetBundleInfos.Length];
            for (int i = 0; i < AssetBundleInfos.Length; i++)
            {
                bundleNames[i] = AssetBundleInfos[i].Name;
            }
            return bundleNames;
        }

        public string GetAssetBundleHash(string assetBundleName)
        {
            if (_infos.TryGetValue(assetBundleName, out var info))
            {
                return info.Hash128;
            }
            return "";
        }

        public string[] GetDirectDependencies(string assetBundleName)
        {
            if (_infos.TryGetValue(assetBundleName, out var info))
            {
                return info.Dependencies;
            }
            return null;
        }

        public string[] GetAllDependencies(string assetBundleName)
        {
            //Debug.Log("get0 deps " + assetBundleName);
            if (_infos.TryGetValue(assetBundleName, out var info))
            {
                if (info.AllDependencies == null)
                {
                    var allDeps = new List<string>();
                    Internal_GetAllDependencies(assetBundleName, allDeps);
                    info.AllDependencies = allDeps.ToArray();
                }
                //Debug.Log("get1 deps " + assetBundleName);
                return info.AllDependencies;
            }
            return null;
        }

        private void Internal_GetAllDependencies(string assetBundleName, List<string> result)
        {
            if (!_infos.TryGetValue(assetBundleName, out var info))
                return;
            if (info.Dependencies != null && info.Dependencies.Length > 0)
            {
                for (int i = 0; i < info.Dependencies.Length; i++)
                {
                    //Debug.Log("get2 depsdep " + info.Dependencies[i]);
                    Internal_GetAllDependencies(info.Dependencies[i], result);
                }
            }
            result.Add(assetBundleName);
        }
    }
}
