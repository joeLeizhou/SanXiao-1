using Orcas.Csv;
using UnityEngine.Scripting;

[Preserve]
public class CsvAtlas : ICsvData<int>
{
    [Preserve]
    public int ID { get; set; }
    [Preserve]
    public string Path { get; set; }
    [Preserve]
    public bool Saved { get; set; }
        
    public CsvAtlas()
    {

    }
}

