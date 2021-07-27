using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class Links
    {
        [JsonPropertyName("self")]
        public Self Self { get; set; }
    }
}
