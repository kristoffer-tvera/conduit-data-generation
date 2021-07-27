using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class ConduitIndexResponse
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("conduits")]
        public List<Conduit> Conduits { get; set; }
    }

    public class Conduit
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("name")]
        public Name Names { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
