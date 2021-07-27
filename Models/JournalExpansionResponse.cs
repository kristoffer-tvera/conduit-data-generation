using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class JournalExpansionResponse
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("dungeons")]
        public List<Dungeon> Dungeons { get; set; }

        [JsonPropertyName("raids")]
        public List<Raid> Raids { get; set; }
    }

    public class Dungeon
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class Raid
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
