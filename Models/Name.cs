using System.Text.Json.Serialization;

namespace ConduitData.Models
{
    public class Name
    {
        [JsonPropertyName("en_US")]
        public string EnUS { get; set; }

        [JsonPropertyName("es_MX")]
        public string EsMX { get; set; }

        [JsonPropertyName("pt_BR")]
        public string PtBR { get; set; }

        [JsonPropertyName("de_DE")]
        public string DeDE { get; set; }

        [JsonPropertyName("en_GB")]
        public string EnGB { get; set; }

        [JsonPropertyName("es_ES")]
        public string EsES { get; set; }

        [JsonPropertyName("fr_FR")]
        public string FrFR { get; set; }

        [JsonPropertyName("it_IT")]
        public string ItIT { get; set; }

        [JsonPropertyName("ru_RU")]
        public string RuRU { get; set; }

        [JsonPropertyName("ko_KR")]
        public string KoKR { get; set; }

        [JsonPropertyName("zh_TW")]
        public string ZhTW { get; set; }

        [JsonPropertyName("zh_CN")]
        public string ZhCN { get; set; }
    }
}
