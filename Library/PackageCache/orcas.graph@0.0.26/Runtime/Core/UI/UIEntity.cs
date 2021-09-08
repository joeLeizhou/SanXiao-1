using Newtonsoft.Json;

namespace Orcas.Graph.Core
{
    [UnityEngine.Scripting.Preserve]
    public abstract class UIEntity
    {
        [JsonProperty("id")]
        [UnityEngine.Scripting.Preserve]
        public int ID { get; set; }
    }
}
