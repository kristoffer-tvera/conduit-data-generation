using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class ConduitResponse
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("item")]
        public Item Item { get; set; }

    }
}
