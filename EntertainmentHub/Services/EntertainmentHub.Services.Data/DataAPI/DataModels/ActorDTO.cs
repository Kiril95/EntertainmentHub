namespace EntertainmentHub.Services.Data.DataAPI.DataModels
{
    using System.Text.Json.Serialization;

    public class ActorDTO
    {
        public string Name { get; set; }

        public string Biography { get; set; }

        public int Gender { get; set; }

        public string Birthday { get; set; }

        public string Deathday { get; set; }

        [JsonPropertyName("place_of_birth")]
        public string Birthplace { get; set; }

        [JsonPropertyName("profile_path")]
        public string Photo { get; set; }

        public double Popularity { get; set; }
    }
}
