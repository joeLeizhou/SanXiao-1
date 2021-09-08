using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orcas.Resources.Interface;
using Orcas.Core.Tools;
using UnityEngine;

namespace Orcas.Resources
{
    internal class Asset
    {
        internal string Path;
        internal Object Value;
        internal Object[] Values;
        internal float NeedRemoveTime;
    }

    public partial class ResourcesManager
    {
        private ILoaderBridge _loader;
        private readonly Dictionary<string, Asset> AssetsMap = new Dictionary<string, Asset>();
        private readonly BinaryHeap<Asset> AssetHeap = new BinaryHeap<Asset>((a, b) =>
        {
            return a.NeedRemoveTime < b.NeedRemoveTime;
        });
        private int[] _preLoadAssetTaskCount = new int[5];
        private Task[] _preLoadAssetTasks = new Task[5];
        private int _preLoadMaxTaskCount = 5;
        /// <summary>
        /// 正在预加载资源
        /// </summary>
        private bool _isPreLoading;
        /// <summary>
        /// 正在异步卸载资源
        /// </summary>
        private bool _isUnloading;

        /// <summary>
        /// 主要用于避免资源在加载的过程中调用销毁方法
        /// </summary>
        private readonly Dictionary<string, int> AssetLock = new Dictionary<string, int>();
        /// <summary>
        /// 记录需要移除的资源
        /// </summary>
        private readonly HashSet<string> NeedRemoveAssets = new HashSet<string>();
        /// <summary>
        /// 资源缓存的最大时间
        /// </summary>
        public float MaxCacheTime = 300;

        /// <summary>
        /// 预加载时最大线程数限制
        /// </summary>
        public int PreLoadMaxTaskCount
        {
            get => _preLoadMaxTaskCount;
            set
            {
                if (_isPreLoading)
                {
                    Debug.LogError("正在预加载过程中，不能修改线程限制参数!");
                    return;
                }
                _preLoadAssetTaskCount = new int[value];
                _preLoadAssetTasks = new Task[value];
                _preLoadMaxTaskCount = value;
            }
        }

        private async Task<Asset> AsyncLoadAndCacheAsset(string path, bool isAll = false)
        {
            Asset asset = null;
            if (AssetsMap.ContainsKey(path))
            {
                asset = AssetsMap[path];
                asset.NeedRemoveTime = Time.time + MaxCacheTime;
                AssetHeap.Update(AssetsMap[path], false);
            }
            else
            {
                asset = AssetsMap[path] = new Asset
                {
                    Path = path,
                    NeedRemoveTime = Time.time + MaxCacheTime
                };
                AssetHeap.Push(asset);
            }

            if (isAll == false)
            {
                if (asset.Value == null)
                {
                    LockPath(path);
                    asset.Value = await _loader.AsyncLoadAsset(path);
                    UnLockPath(path);
                }
            }
            else
            {
                LockPath(path);
                asset.Values = await _loader.AsyncLoadAllAssets(path);
                UnLockPath(path);
            }

            return asset;
        }

        private Asset LoadAndCacheAsset(string path, bool isAll = false)
        {
            Asset asset = null;
            if (AssetsMap.ContainsKey(path))
            {
                asset = AssetsMap[path];
                asset.NeedRemoveTime = Time.time + MaxCacheTime;
                AssetHeap.Update(AssetsMap[path], false);
            }
            else
            {
                asset = AssetsMap[path] = new Asset
                {
                    Path = path,
                    NeedRemoveTime = Time.time + MaxCacheTime
                };
                AssetHeap.Push(asset);
            }

            if (isAll == false)
            {
                if (asset.Value == null)
                {
                    asset.Value = _loader.LoadAsset(path);
                }
            }
            else
            {
                asset.Values = _loader.LoadAllAssets(path);
            }

            return asset;
        }

        private T LoadAsset<T>(string path, bool cache = true) where T : Object
        {
            if (cache == false)
            {
                return _loader.LoadAsset(path) as T;
            }
            else
            {
                return LoadAndCacheAsset(path).Value as T;
            }
        }

        private T[] LoadAllAsset<T>(string path, bool cache = true) where T : Object
        {
            if (cache == false)
            {
                return _loader.LoadAllAssets(path).OfType<T>().ToArray();
            }
            else
            {
                return LoadAndCacheAsset(path, true).Values.OfType<T>().ToArray();
            }
        }

        private void LockPath(string path)
        {
            if (AssetLock.ContainsKey(path) == false)
            {
                AssetLock.Add(path, 1);
            }
            else
            {
                AssetLock[path]++;
            }
        }

        private void UnLockPath(string path)
        {
            var count = AssetLock[path];
            if (count == 1) AssetLock.Remove(path);
            else AssetLock[path] = count - 1;
        }

        private async Task AsyncPreLoadAssets(IReadOnlyList<string> paths, int startIndex, int count)
        {
            var needLoad = new List<string>();
            for (var i = startIndex; i < paths.Count && i < count + startIndex; i++)
            {
                if (AssetsMap.ContainsKey(paths[i])) continue;
                needLoad.Add(paths[i]);
                LockPath(paths[i]);
            }

            var objs = await _loader.AsyncLoadAssets(needLoad.ToArray());

            for (var i = 0; i < objs.Length; i++)
            {
                AssetsMap[needLoad[i]] = new Asset
                {
                    Path = needLoad[i],
                    Value = objs[i],
                    NeedRemoveTime = Time.time + MaxCacheTime
                };
                AssetHeap.Push(AssetsMap[needLoad[i]]);
                UnLockPath(needLoad[i]);
            }
        }

        public ResourcesManager(ILoaderBridge loader)
        {
            NeedRemoveAssets.Clear();
            AssetsMap.Clear();
            AssetHeap.Clear();
            AssetLock.Clear();
            _isUnloading = false;
            _isPreLoading = false;
            _loader = loader;
        }

        /// <summary>
        /// 初始化资源配置
        /// </summary>
        /// <param name="callback"></param>
        public void Init(System.Action callback)
        {
            _loader.Init(callback);
        }

        /// <summary>
        /// 加载（重新加载）资源配置文件
        /// 在初始化或者分包下载之后调用
        /// </summary>
        public void LoadFileLists()
        {
            _loader.LoadFileLists();
        }
        /// <summary>
        /// 卸载没有用的资源
        /// 任务完成前不允许多次调用（不会抛出异常，但是调用会被忽略掉）
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UnloadUnusedAsset()
        {
            if (_isUnloading)
            {
                Debug.LogError("已经在卸载过程中了！该方法不允许同时多个调用！");
                return false;
            }
            _isUnloading = true;
            while (AssetHeap.Count > 0)
            {
                var asset = AssetHeap.Peek();
                if (!(asset.NeedRemoveTime < Time.time) || AssetLock.ContainsKey(asset.Path)) break;
                AssetHeap.Pop();
                AssetsMap.Remove(asset.Path);
                NeedRemoveAssets.Add(asset.Path);
            }
            await _loader.AsyncUnloadUnusedAssets();
            _isUnloading = false;
            return true;
        }

        public async Task<bool> ClearAllAsset()
        {
            if (_isUnloading)
            {
                Debug.LogError("已经在卸载过程中了！该方法不允许同时多个调用！");
                return false;
            }
            _isUnloading = true;
            NeedRemoveAssets.Clear();
            AssetsMap.Clear();
            AssetHeap.Clear();
            AssetLock.Clear();
            _isUnloading = false;
            _isPreLoading = false;
            await _loader.AsyncUnloadAllAssets();
            _isUnloading = false;
            return true;
        }

        /// <summary>
        /// 卸载被缓存下来的资源
        /// </summary>
        /// <param name="path"></param>
        public bool UnLoadAsset(string path)
        {
            if (!AssetsMap.ContainsKey(path)) return true;
            if (AssetLock.ContainsKey(path)) return false;
            _loader.UnloadAsset(AssetsMap[path].Value);
            AssetHeap.Remove(AssetsMap[path]);
            AssetsMap.Remove(path);
            return true;
        }

        /// <summary>
        /// 卸载没有被缓存的资源
        /// </summary>
        /// <param name="asset">资源文件</param>
        public void UnLoadAsset(Object asset)
        {
            _loader.UnloadAsset(asset);
        }

        /// <summary>
        /// 预加载资源
        /// 异步方法
        /// 所有通过此方法加载的资源都将被缓存下来
        /// 任务完成前不允许多次调用（不会抛出异常，但是调用会被忽略掉）
        /// </summary>
        /// <param name="paths"></param>
        public async Task AsyncPreLoadAssets(string[] paths)
        {
            if (_isPreLoading)
            {
                Debug.LogError("已经在预加载过程中了！该方法不允许同时多个调用！");
                return;
            }
            _isPreLoading = true;
            var count = paths.Length / _preLoadMaxTaskCount;
            var split = paths.Length % _preLoadMaxTaskCount;
            for (var i = 0; i < _preLoadMaxTaskCount; i++)
            {
                _preLoadAssetTaskCount[i] = count + (i < split ? 1 : 0);
            }

            var s = 0;
            for (var i = 0; i < PreLoadMaxTaskCount; i++)
            {
                if (_preLoadAssetTaskCount[i] == 0)
                {
                    _preLoadAssetTasks[i] = Task.CompletedTask;
                    continue;
                }
                _preLoadAssetTasks[i] = AsyncPreLoadAssets(paths, s, _preLoadAssetTaskCount[i]);
                s += _preLoadAssetTaskCount[i];
            }

            await Task.WhenAll(_preLoadAssetTasks);

            _isPreLoading = false;
        }

        /// <summary>
        /// 异步加载原始资源文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> AsyncLoadOriginalAsset<T>(string path, bool cache = true) where T : Object
        {
            if (cache)
            {
                return (await AsyncLoadAndCacheAsset(path)).Value as T;
            }
            else
            {
                LockPath(path);
                var prefab = await _loader.AsyncLoadAsset(path);
                UnLockPath(path);
                return prefab as T;
            }
        }

        public async Task<T[]> AsyncLoadAllOriginalAsset<T>(string path, bool cache = true) where T : Object
        {
            if (cache)
            {
                return (await AsyncLoadAndCacheAsset(path, true)).Values.OfType<T>().ToArray();
            }
            else
            {
                LockPath(path);
                var objs = await _loader.AsyncLoadAllAssets(path);
                UnLockPath(path);
                return objs.OfType<T>().ToArray();
            }
        }



        /// <summary>
        /// 异步加载实例化后对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> AsyncInstantiatedObject<T>(string path, bool cache = true) where T : Object
        {
            var prefab = await AsyncLoadOriginalAsset<T>(path, cache);
            return Object.Instantiate(prefab);
        }
    }
}