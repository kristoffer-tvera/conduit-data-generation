using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class JournalInstanceResponse
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("encounters")]
        public List<Encounter> Encounters { get; set; }

    }

    public class Encounter
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
