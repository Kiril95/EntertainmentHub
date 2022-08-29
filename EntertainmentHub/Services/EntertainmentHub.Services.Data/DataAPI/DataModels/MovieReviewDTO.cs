namespace EntertainmentHub.Services.Data.DataAPI.DataModels
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class MovieReviewDTO
    {
        [JsonPropertyName("id")]
        public int MovieId { get; set; }

        public int Page { get; set; }

        [JsonPropertyName("results")]
        public ICollection<ReviewDTO> Reviews { get; set; }
    }

    public class ReviewDTO
    {
        [JsonPropertyName("author")]
        public string AuthorReviewName { get; set; }

        [JsonPropertyName("author_details")]
        public ReviewAuthorDTO AuthorDetails { get; set; }

        public string Content { get; set; }
    }

    public class ReviewAuthorDTO
    {
        public string Name { get; set; }

        public string Username { get; set; }

        [JsonPropertyName("avatar_path")]
        public string Avatar { get; set; }

        public double? Rating { get; set; }
    }
}
