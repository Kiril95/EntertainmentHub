namespace EntertainmentHub.Services.Data.DataAPI.DataModels
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class TrailerDTO
    {
        [JsonPropertyName("results")]
        public ICollection<PathDTO> Trailers { get; set; }
    }

    public class PathDTO
    {
        public string Name { get; set; }

        public string Site { get; set; }

        public string Type { get; set; }

        [JsonPropertyName("key")]
        public string Path { get; set; }

        public bool Official { get; set; }
    }
}
