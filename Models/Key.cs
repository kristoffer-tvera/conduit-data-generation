using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class Key
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }
}
