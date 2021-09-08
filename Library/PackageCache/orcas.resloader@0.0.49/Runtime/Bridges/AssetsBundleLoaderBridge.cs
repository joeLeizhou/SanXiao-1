using Orcas.Resources.Interface;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Orcas.AssetBuilder;

namespace Orcas.Resources
{
    public class AssetsBundleLoaderBridge : ILoaderBridge
    {
        private Dictionary<string, AssetBundle> _assetBundleDic = new Dictionary<string, AssetBundle>(256); //加载过的ab缓存
        private List<string> _resourceLoadLockList = new List<string>(64); //防止重复加载 
        private Dictionary<Object, AssetBundle> _objectAssetBundleDic = new Dictionary<Object, AssetBundle>(256); //加载过的ab依赖
        private AssetBundleManifest _manifest = null; // 主ab依赖配置
        private PackAssetBundleManifest _packsManifest = null; // 分包ab依赖配置
        private Dictionary<string, CsvFileListInfo> _fileList, _packsFileList; // 主ab和分包ab文件配置
        public void Init(System.Action callback)
        {
            _assetBundleDic.Clear();
            _objectAssetBundleDic.Clear();
            _resourceLoadLockList.Clear();
            LoadFileLists();
            _ = LoadManifestAsync(callback);
        }

        public void LoadFileLists()
        {
            _fileList = CsvFileListUtil.LoadDicAtPath(PathConst.DestPath + PathConst.FileListName);
            Debug.Log("filelist count " + _fileList.Count);
            _packsFileList = CsvFileListUtil.LoadDicAtPath(PathConst.DestPath + PathConst.PacksFileListName);
            _packsManifest = PackAssetBundleManifest.Load(PathConst.DestPath + PathConst.PacksManifestName);
            _packsManifest?.Init();
            Debug.Log("packfile count " + _packsManifest?.Infos.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetABName(string path)
        {
            return Path.GetFileNameWithoutExtension(path).ToLower();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetBundleName(string name)
        {
            if (_fileList.TryGetValue(name, out var info))
                return info.BundleName;
            if (_packsFileList != null && _packsFileList.TryGetValue(name, out var info1))
                return info1.BundleName;
            return null;
        }

        public Object LoadAsset(string path)
        {
            var abName = GetABName(path);
            return LoadTObjectInAssetsBundle<Object>(abName, abName);
        }

        public void LoadSceneAsset(string path)
        {
            var abName = GetABName(path);
            LoadSceneInAssetsBundle(abName, abName);
        }

        public Object[] LoadAllAssets(string path)
        {
            var abName = GetABName(path);
            return LoadAllTObjectInAssetBundle<Object>(abName, abName);
        }

        public Task<Object> AsyncLoadAsset(string path)
        {
            var abName = GetABName(path);
            return LoadTObjectInAssetBundleAsync<Object>(abName, abName);
        }

        public Task<Object[]> AsyncLoadAllAssets(string path)
        {
            var abName = GetABName(path);
            return LoadTAllObjectInAssetBundleAsync<Object>(abName, abName);
        }

        public async Task<Object[]> AsyncLoadAssets(string[] paths)
        {
            var results = new AssetBundleRequest[paths.Length];
            var ret = new Object[paths.Length];
            var completeList = new bool[paths.Length];
            var index = 0;
            for (int i = 0; i < paths.Length; i++)
            {
                if (string.IsNullOrEmpty(paths[i])) return null;
                await LoadDependenciesBundleAsync(paths[i]);
                AssetBundle bundle = await GetAssetBundleAsync(paths[i]);
                results[i] = bundle.LoadAssetAsync(paths[i]);
                results[i].completed += (op) =>
                {
                    completeList[index] = true;
                    index++;
                };
            }
            var task = Task.Run(async () =>
            {
                while (true)
                {
                    var finish = true;
                    for (int i = 0; i < completeList.Length; i++)
                    {
                        finish &= completeList[i];
                    }
                    if (finish == true)
                    {
                        break;
                    }
                    else
                    {
                        await Task.Delay(20);
                    }
                }
            });
            await task;
            for (var i = 0; i < results.Length; i++)
            {
                ret[i] = results[i].asset;
            }

            return ret;
        }

        public Task AsyncUnloadUnusedAssets()
        {
            return Task.Delay(20);
        }

        public Task AsyncUnloadAllAssets()
        {
            foreach (var bundle in _assetBundleDic)
            {
                bundle.Value.Unload(false);
            }
            _assetBundleDic.Clear();
            _objectAssetBundleDic.Clear();
            _resourceLoadLockList.Clear();
            _manifest = null;
            _packsManifest = null;
            _fileList.Clear();
            if (_packsFileList != null) _packsFileList.Clear();
            return Task.Delay(20);
        }

        public void UnloadAsset(Object obj)
        {
            //Resources.UnloadAsset仅能释放非GameObject和Component的资源，比如Texture、Mesh等真正的资源。对于由Prefab加载出来的Object或Component，则不能通过该函数来进行释放。
            if (_objectAssetBundleDic.ContainsKey(obj))
            {
                _objectAssetBundleDic[obj].Unload(false);
                _objectAssetBundleDic.Remove(obj);
            }
        }

        public async Task LoadManifestAsync(System.Action callback = null)
        {
            string path = PathConst.DestPath + PathConst.ManifestName;
            Debug.Log("load " + path);
            if (_manifest != null)
                AssetBundle.UnloadAllAssetBundles(true);
            {
                string url = path;
                var result = AssetBundle.LoadFromFileAsync(url);
                var finish = false;
                var task = Task.Run(async () =>
                {
                    while (finish == false)
                    {
                        await Task.Delay(20);
                    }
                });
                result.completed += (op) => { finish = true; };
                await task;

                var bundle = result.assetBundle;
                if (bundle != null)
                {
                    var result1 = bundle.LoadAssetAsync("AssetBundleManifest");
                    var finish1 = false;
                    var task1 = Task.Run(async () =>
                    {
                        while (finish1 == false)
                        {
                            await Task.Delay(20);
                        }
                    });
                    result1.completed += (op) => { finish1 = true; };
                    await task1;

                    _manifest = (AssetBundleManifest)result1.asset;
                    Debug.Log(" manifest loaded");
                    bundle.Unload(false);
                    callback?.Invoke();
                }
                else
                {
                    Debug.LogError("load failed " + url);
                }
            }
        }

        //--同步加载ab的obj
        private T LoadTObjectInAssetsBundle<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            LoadDependenciesBundle(bundleName, false);
            if (string.IsNullOrEmpty(assetName)) return null;
            AssetBundle bundle = GetAssetBundle(bundleName);
            T asset = bundle.LoadAsset<T>(assetName);
            if (!_objectAssetBundleDic.ContainsKey(asset))
            {
                _objectAssetBundleDic.Add(asset, bundle);
            }
            return asset;
        }
        private void LoadSceneInAssetsBundle(string bundleName, string assetName)
        {
            LoadDependenciesBundle(bundleName, false);
            if (string.IsNullOrEmpty(assetName)) return;
            AssetBundle bundle = GetAssetBundle(bundleName);
        }

        //--同步加载ab的obj的所有obj
        private T[] LoadAllTObjectInAssetBundle<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            LoadDependenciesBundle(bundleName, false);
            if (string.IsNullOrEmpty(assetName)) return null;
            AssetBundle bundle = GetAssetBundle(bundleName);
            T[] assets = bundle.LoadAllAssets<T>();
            return assets;
        }

        //--异步加载ab的obj
        private async Task<T> LoadTObjectInAssetBundleAsync<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            await LoadDependenciesBundleAsync(bundleName);
            if (string.IsNullOrEmpty(bundleName)) return null;
            AssetBundle bundle = await GetAssetBundleAsync(bundleName);
            var result = bundle.LoadAssetAsync(assetName);
            var finish = false;
            var task = Task.Run(async () =>
            {
                while (finish == false)
                {
                    await Task.Delay(20);
                }
            });
            result.completed += (op) => { finish = true; };
            await task;
            return result.asset as T;
        }

        //--异步加载ab的obj的所有obj
        private async Task<T[]> LoadTAllObjectInAssetBundleAsync<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            await LoadDependenciesBundleAsync(bundleName);
            if (string.IsNullOrEmpty(bundleName)) return null;
            AssetBundle bundle = await GetAssetBundleAsync(bundleName);
            var result = bundle.LoadAllAssetsAsync<T>();
            var finish = false;
            var task = Task.Run(async () =>
            {
                while (finish == false)
                {
                    await Task.Delay(20);
                }
            });
            result.completed += (op) => { finish = true; };
            await task;
            return result.allAssets as T[];
        }

        //--同步加载ab
        private AssetBundle GetAssetBundle(string bundleName)
        {
            if (string.IsNullOrEmpty(bundleName)) return null;
            bundleName = bundleName.ToLower();
            AssetBundle bundle = _assetBundleDic.ContainsKey(bundleName) ? _assetBundleDic[bundleName] : null;
            if (bundle == null)
            {
                try
                {
                    //Debug.Log("load ab " + bundleName);
                    string url = PathConst.DestPath + GetBundleName(bundleName);
                    bundle = AssetBundle.LoadFromFile(url);
                    _assetBundleDic.Add(bundleName, bundle);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(this.GetType() + "->GetAssetBundle  " + "\ncatch exception:" + ex);
                }
            }
            return bundle;
        }

        //--异步加载ab
        private async Task<AssetBundle> GetAssetBundleAsync(string bundleName)
        {
            if (string.IsNullOrEmpty(bundleName)) return null;
            bundleName = bundleName.ToLower();
            AssetBundle bundle = _assetBundleDic.ContainsKey(bundleName) ? _assetBundleDic[bundleName] : null;
            if (bundle == null)
            {
                try
                {
                    string url = PathConst.DestPath + GetBundleName(bundleName);
                    if (_resourceLoadLockList.Contains(bundleName))
                    {
                        return null;
                    }
                    else
                    {
                        _resourceLoadLockList.Add(bundleName);
                    }

                    var result = AssetBundle.LoadFromFileAsync(url);
                    var finish = false;
                    var task = Task.Run(async () =>
                    {
                        while (finish == false)
                        {
                            await Task.Delay(20);
                        }
                    });
                    result.completed += (op) => { finish = true; };
                    await task;
                    _resourceLoadLockList.Remove(bundleName);
                    _assetBundleDic.Add(bundleName, result.assetBundle);
                    bundle = result.assetBundle;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(this.GetType() + "->GetAssetBundleAsync  " + "\ncatch exception:" + ex);
                }
            }
            return bundle;
        }


        /// <summary>
        /// 异步加载依赖的ab
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private async Task LoadDependenciesBundleAsync(string bundleName)
        {
            string[] dpName = _manifest.GetAllDependencies(bundleName);
            if ((dpName == null || dpName.Length == 0) && _packsManifest != null)
                dpName = _packsManifest.GetAllDependencies(bundleName);
            if (dpName == null)
                return;
            for (int i = 0; i < dpName.Length; i++)
            {
                await GetAssetBundleAsync(dpName[i]);
            }
        }
        /// <summary>
        /// 同步加载依赖的ab
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private void LoadDependenciesBundle(string bundleName, bool isAsync)
        {
            string[] dpName = _manifest.GetAllDependencies(bundleName);
            if ((dpName == null || dpName.Length == 0) && _packsManifest != null)
                dpName = _packsManifest.GetAllDependencies(bundleName);
            if (dpName == null)
                return;
            for (int i = 0; i < dpName.Length; i++)
            {
                GetAssetBundle(dpName[i]);
            }
        }
    }
}