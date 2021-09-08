namespace Orcas.Resources
{
    public class ResourceLoaderManager
    {

#if UNITY_EDITOR && !ASSETSBUNDLE_TEST
        /// <summary>
        /// 加载包外资源
        /// </summary>
        public static ResourcesManager AssetLoader = new ResourcesManager(new AssetDataBaseLoaderBridge());
#else
        public static ResourcesManager AssetLoader = new ResourcesManager(new AssetsBundleLoaderBridge());
#endif
        /// <summary>
        /// 加载包内resource目录下的东西
        /// </summary>
        public static ResourcesManager ResourceLoader = new ResourcesManager(new AssetResourcesLoaderBridge());

        public static async void ClearAllAsset()
        {
            UnityEngine.Debug.Log("start clear");
            await AssetLoader.ClearAllAsset();
            await ResourceLoader.ClearAllAsset();
            UnityEngine.Debug.Log("end clear");
        }
    }
}
