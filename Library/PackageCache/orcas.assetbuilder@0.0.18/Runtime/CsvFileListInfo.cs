using Orcas.Csv;
using UnityEngine.Scripting;

namespace Orcas.AssetBuilder
{
    [Preserve]
    public class CsvFileListInfo : ICsvData<string>
    {
        [Preserve]
        public string ID { get; set; }
        [Preserve]
        public string BundleName { get; set; }
        [Preserve]
        public float Size { get; set; }
        [Preserve]
        public bool Saved { get; set; }
        public CsvFileListInfo()
        {
        }

        public CsvFileListInfo(string id, string md5, float size)
        {
            this.ID = id;
            this.BundleName = md5;
            this.Size = size;
        }
    }
}