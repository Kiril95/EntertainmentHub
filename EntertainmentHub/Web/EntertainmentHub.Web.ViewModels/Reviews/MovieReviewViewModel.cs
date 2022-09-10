namespace EntertainmentHub.Web.ViewModels.Reviews
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieReviewViewModel : IMapFrom<MovieReview>
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public string AuthorName { get; set; }

        public string AuthorUsername { get; set; }

        public string AvatarPath { get; set; }

        public string Content { get; set; }
    }
}
