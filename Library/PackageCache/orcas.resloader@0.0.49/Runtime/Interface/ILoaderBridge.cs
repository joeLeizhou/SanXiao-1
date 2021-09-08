using System.Threading.Tasks;
using UnityEngine;

namespace Orcas.Resources.Interface
{
    public interface ILoaderBridge
    {
        void Init(System.Action onManifestLoaded);
        void LoadFileLists();
        Object LoadAsset(string path);
        void LoadSceneAsset(string path);
        Object[] LoadAllAssets(string path);
        void UnloadAsset(Object obj);
        Task<Object> AsyncLoadAsset(string path);
        Task<Object[]> AsyncLoadAssets(string[] paths);
        Task<Object[]> AsyncLoadAllAssets(string path);
        Task AsyncUnloadUnusedAssets();
        Task AsyncUnloadAllAssets();
    }
}