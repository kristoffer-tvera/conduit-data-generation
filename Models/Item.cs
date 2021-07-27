using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class Item
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
