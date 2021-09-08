using System.Collections.Generic;
using Orcas.Csv;
using UnityEngine;

namespace Orcas.Resources
{
    public class Atlas
    {
        public int Id;
        private readonly Dictionary<string, Sprite> _sprites;

        private Atlas(int id, IEnumerable<Sprite> sprites)
        {
            Id = id;
            _sprites = new Dictionary<string, Sprite>();
            foreach (var sprite in sprites)
            {
                _sprites.Add(sprite.name, sprite);
            }
        }

        /// <summary>
        /// 获取图集中的精灵，如果为空抛出异常
        /// </summary>
        /// <param name="sprName"></param>
        /// <returns></returns>
        public Sprite GetSprite(string sprName)
        {
            return _sprites[sprName];
        }

        private static Dictionary<int, Atlas> _atlases = new Dictionary<int, Atlas>();

        /// <summary>
        /// 获取图集，如果图集不存在则创建一个新图集
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sprites"></param>
        /// <returns></returns>
        public static Atlas GetOrCreateAtlas(int id, Sprite[] sprites)
        {
            if (_atlases.ContainsKey(id))
            {
                return _atlases[id];
            }
            return _atlases[id] = new Atlas(id, sprites);
        }
        
        /// <summary>
        /// 获取图集，如果图集不存在则创建一个新的图集
        /// 从表CsvAtlas中记载图集
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Atlas GetOrCreateAtlas(int id)
        {
            if (_atlases.ContainsKey(id))
            {
                return _atlases[id];
            }
            var csv = CsvLoader<int>.GetCsvData<CsvAtlas>(id);
            var sprites = ResourceLoaderManager.AssetLoader.LoadAllOriginalAsset<Sprite>(csv.Path);
            return _atlases[id] = new Atlas(id, sprites);
        }

        /// <summary>
        /// 获取图集，如果找不到该图集则报错返回空
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Atlas GetAtlas(int id)
        {
            if (_atlases.ContainsKey(id)) return _atlases[id];
            Debug.LogError($"[Atlas] 找不到图集!id:{id}");
            return null;
        }

        /// <summary>
        /// 移除图集
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool RemoveAtlas(int id)
        {
            return _atlases.Remove(id);
        }
    }
}