using System.Text.Json.Serialization;

namespace paiementService.Models.Commands
{
    public class DebitHistory
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("movieId")]
        public int MovieId { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
