namespace EntertainmentHub.Services.Data.DataAPI.DataModels
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class SlideshowDTO
    {
        public ICollection<SlideDTO> Backdrops { get; set; }
    }

    public class SlideDTO
    {
        [JsonPropertyName("file_path")]
        public string FilePath { get; set; }

        [JsonPropertyName("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonPropertyName("iso_639_1")]
        public string ISO { get; set; }
    }
}
