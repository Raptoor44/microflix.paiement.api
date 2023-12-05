using System.Text.Json.Serialization;

namespace paiementService.Models
{
    public class Logs
    {
        [JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }

        [JsonPropertyName("log")]
        public string Log { get; set; }
    }
}
