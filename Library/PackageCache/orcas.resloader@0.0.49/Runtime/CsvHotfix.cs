using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Csv;
using UnityEngine.Scripting;

[assembly: Preserve]
namespace Orcas.Resources
{
    [Preserve]
    public class CsvHotfix : ICsvData<string>
    {
        [Preserve]
        public string ID { get; set; }
        [Preserve]
        public string HotVer { get; set; }
        [Preserve]
        public float Size { get; set; }
        [Preserve]
        public bool ForceUpdate { get; set; }
        [Preserve]
        public bool Saved { get; set; }
        public CsvHotfix()
        {

        }
        public CsvHotfix(string id, string hotV, float size, bool force)
        {
            ID = id;
            HotVer = hotV;
            Size = size;
            ForceUpdate = force;
        }
        public override string ToString()
        {
            return $"id:{ID},hotv:{HotVer},size:{Size},force:{ForceUpdate}";
        }
    }
}