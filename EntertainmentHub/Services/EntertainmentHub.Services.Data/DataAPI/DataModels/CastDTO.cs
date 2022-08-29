namespace EntertainmentHub.Services.Data.DataAPI.DataModels
{
    using System.Text.Json.Serialization;

    public class CastDTO
    {
        [JsonPropertyName("id")]
        public int ActorId { get; set; }

        [JsonPropertyName("character")]
        public string CharacterName { get; set; }

        [JsonPropertyName("known_for_department")]
        public string Department { get; set; }
    }
}
