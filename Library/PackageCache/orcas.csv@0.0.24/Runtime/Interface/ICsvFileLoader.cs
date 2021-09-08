using UnityEngine;

namespace Orcas.Csv
{
    public interface ICsvFileLoader
    {
        string CsvPath { get; set; }
        TextAsset LoadCsvFile(string file);
    }

    public class DefaultCsvResourcesLoader : ICsvFileLoader
    {
        public string CsvPath { get; set; }
        public TextAsset LoadCsvFile(string file)
        {
            return Resources.Load<TextAsset>(System.IO.Path.Combine(CsvPath, file));
        }
    }
}