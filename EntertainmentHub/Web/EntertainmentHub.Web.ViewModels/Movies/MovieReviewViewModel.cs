namespace EntertainmentHub.Web.ViewModels.Movies
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieReviewViewModel : IMapFrom<MovieReview>
    {
        public int MovieId { get; set; }

        public int ReviewId { get; set; }

        public string AuthorName { get; set; }

        public string AuthorUsername { get; set; }

        public string AvatarPath { get; set; }

        public string Content { get; set; }
    }
}
