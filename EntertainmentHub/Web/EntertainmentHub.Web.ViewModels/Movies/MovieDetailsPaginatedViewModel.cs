namespace EntertainmentHub.Web.ViewModels.Movies
{
    using EntertainmentHub.Web.ViewModels.Comments;

    public class MovieDetailsPaginatedViewModel
    {
        public MovieViewModel Movie { get; set; }

        public PaginatedList<MovieCommentViewModel> Comments { get; set; }

        public int TotalCount { get; set; }
    }
}
