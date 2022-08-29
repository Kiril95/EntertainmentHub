namespace EntertainmentHub.Services.Data.DataAPI.DataModels
{
    using System.Text.Json.Serialization;

    public class CountryDTO
    {
        [JsonPropertyName("iso_3166_1")]
        public string ISO { get; set; }

        public string Name { get; set; }
    }
}