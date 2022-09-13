namespace EntertainmentHub.Web.ViewModels.Reviews
{
    using EntertainmentHub.Web.ViewModels.Movies;

    public class ReviewPaginatedListViewModel
    {
        public MovieSimpleViewModel Movie { get; set; }

        public PaginatedList<MovieReviewViewModel> Reviews { get; set; }

        public int TotalCount { get; set; }

        public int PageResults()
        {
            int pageResults = 0;

            if (this.TotalCount < this.Reviews.PageNumber * 10)
            {
                return pageResults = this.TotalCount;
            }
            else
            {
                return pageResults = this.Reviews.PageNumber * 10;
            }
        }
    }
}
