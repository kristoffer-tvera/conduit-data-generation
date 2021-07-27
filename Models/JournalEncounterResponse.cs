using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class JournalEncounterResponse
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }


        [JsonPropertyName("items")]
        public List<SuperItem> Items { get; set; }

    }

    public class SuperItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("item")]
        public Item Item { get; set; }
    }

}
