using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class Self
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }
}
