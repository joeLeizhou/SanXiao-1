using UnityEngine;
using Orcas.Csv;
using System.IO;

namespace Orcas.Resources
{
    
    public class DefaultCsvFileLoader : ICsvFileLoader
    {
        /// <summary>
        /// 加载资源方式
        /// </summary>
        private bool _useResourcesLoader = false;
        
        /// <summary>
        /// 文件路径
        /// </summary>
        public string CsvPath { get; set; } = "Assets/External/Csv/";
        
        public TextAsset LoadCsvFile(string file)
        {
            if (_useResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<TextAsset>(Path.Combine(CsvPath, file));
            }
            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<TextAsset>(Path.Combine(CsvPath, file));
        }

        public DefaultCsvFileLoader(string csvPath = "Assets/External/Csv/", bool useResourcesLoader = false)
        {
            SetLoaderParams(csvPath, useResourcesLoader);
        }

        public void SetLoaderParams(string csvPath = "Assets/External/Csv/", bool useResourcesLoader = false)
        {
            _useResourcesLoader = useResourcesLoader;
            CsvPath = csvPath;
        }
    }
}

