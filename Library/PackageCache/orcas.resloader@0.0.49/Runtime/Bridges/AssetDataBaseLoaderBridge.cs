using System.Threading.Tasks;
using Orcas.Resources.Interface;
using UnityEngine;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;

namespace Orcas.Resources
{
    public class AssetDataBaseLoaderBridge : ILoaderBridge
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPath(string path)
        {
            if (path.StartsWith("Assets/", System.StringComparison.Ordinal) == false)
                path = "Assets/Resources/" + path;

            var findAssets = AssetDatabase.FindAssets(System.IO.Path.GetFileName(path), new string[] { System.IO.Path.GetDirectoryName(path) });
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            foreach (var asset in findAssets)
            {
                var tempPath = AssetDatabase.GUIDToAssetPath(asset);
                if (System.IO.Path.GetFileNameWithoutExtension(tempPath) == fileName)
                    return tempPath;
            }
            Debug.LogError("can not find " + path);
            return path;
        }

        public Task<Object> AsyncLoadAsset(string path)
        {
            var result = AssetDatabase.LoadMainAssetAtPath(GetPath(path));
            return Task.Run(() =>
            {
                return result;
            });
        }

        public Task<Object[]> AsyncLoadAssets(string[] paths)
        {
            var results = new Object[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                results[i] = AssetDatabase.LoadMainAssetAtPath(GetPath(paths[i]));
            }
            return Task.Run(() =>
            {
                return results;
            });
        }

        public Task AsyncUnloadUnusedAssets()
        {
            return Task.Delay(20);
        }

        public Task AsyncUnloadAllAssets()
        {
            return Task.Delay(20);
        }

        public void Init(System.Action callback)
        {
            callback?.Invoke();
        }

        public void LoadFileLists()
        {

        }

        public Object[] LoadAllAssets(string path)
        {
            return AssetDatabase.LoadAllAssetsAtPath(GetPath(path));
        }

        public Object LoadAsset(string path)
        {
            return AssetDatabase.LoadMainAssetAtPath(GetPath(path));
        }
        public void LoadSceneAsset(string path)
        {

        }

        public async Task<Object[]> AsyncLoadAllAssets(string path)
        {
            //没有异步，假异步
            return AssetDatabase.LoadAllAssetsAtPath(GetPath(path));
        }

        public void UnloadAsset(Object obj)
        {

        }
    }
}
#endif