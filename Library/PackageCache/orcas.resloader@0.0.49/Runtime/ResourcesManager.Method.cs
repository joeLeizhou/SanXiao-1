using System.Threading.Tasks;
using Orcas.Resources.Interface;
using UnityEngine;

namespace Orcas.Resources
{
    public partial class ResourcesManager
    {
        /// <summary>
        /// 加载实例化后的对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache"></param>
        /// <param name="objectPoolCache"></param>
        /// <returns></returns>
        public T LoadInstantiatedObject<T>(string path, bool cache = true, bool objectPoolCache = false) where T : Object
        {
            return Object.Instantiate(LoadAsset<T>(path, cache));
        }

        /// <summary>
        /// 加载资源文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache">是否缓存资源文件，如果未缓存，则需要手动清理资源！</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadOriginalAsset<T>(string path, bool cache = true) where T : Object
        {
            return LoadAsset<T>(path, cache);
        }

        /// <summary>
        /// 加载资源文件及其子资源
        /// </summary>
        /// <typeparam name="T">要先把泛型填成Object，要不然loadAll加载不出来，加载之后再里式转为对应的东西(不知道是不是Unity的坑）</typeparam>
        /// <param name="path">路径</param>
        /// <param name="cache">是否缓存资源文件，如果未缓存，则需要手动清理资源！</param>
        public T[] LoadAllOriginalAsset<T>(string path, bool cache = true) where T : Object
        {
            return LoadAllAsset<T>(path, cache);
        }

        public void LoadSceneAsset(string path)
        {
            _loader.LoadSceneAsset(path);
        }

    }
}
