using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Orcas.Resources.Interface;
using UnityEngine;

namespace Orcas.Resources
{
    public class AssetResourcesLoaderBridge : ILoaderBridge
    {
        public void Init(System.Action callback)
        {
            callback?.Invoke();
        }
        public void LoadFileLists()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPath(string path)
        {
            if (path.StartsWith("Assets/Resources/", System.StringComparison.Ordinal))
                return path.Substring(17);
            return path;
        }

        public Object LoadAsset(string path)
        {
            return UnityEngine.Resources.Load(GetPath(path));
        }
        public void LoadSceneAsset(string path)
        {

        }
        public Object[] LoadAllAssets(string path)
        {
            return UnityEngine.Resources.LoadAll(GetPath(path));
        }

        public void UnloadAsset(Object obj)
        {
            UnityEngine.Resources.UnloadAsset(obj);
        }

        public async Task<Object> AsyncLoadAsset(string path)
        {
            var result = UnityEngine.Resources.LoadAsync(GetPath(path));
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
            return result.asset;
        }

        public async Task<Object[]> AsyncLoadAssets(string[] assetNames)
        {
            var results = new ResourceRequest[assetNames.Length];
            var ret = new Object[assetNames.Length];
            var completeList = new bool[assetNames.Length];
            int index = 0;
            for (var i = 0; i < assetNames.Length; i++)
            {
                results[i] = UnityEngine.Resources.LoadAsync(GetPath(assetNames[i]));
                results[i].completed += (op) => { completeList[index] = true; index++; };
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

        public async Task AsyncUnloadUnusedAssets()
        {
            var result = UnityEngine.Resources.UnloadUnusedAssets();
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
        }

        public async Task AsyncUnloadAllAssets()
        {
            var result = UnityEngine.Resources.UnloadUnusedAssets();
            var finish = false;
            var task = Task.Run(async () =>
            {
                while (finish == false)
                {
                    await Task.Delay(20);
                }
                System.GC.Collect();
                UnityEngine.Debug.Log("gc clear");
            });
            result.completed += (op) => { finish = true; };
            await task;
        }

        public async Task<Object[]> AsyncLoadAllAssets(string path)
        {
            await Task.Delay(20);
            //没有异步，假异步
            return UnityEngine.Resources.LoadAll(GetPath(path));
        }
    }
}