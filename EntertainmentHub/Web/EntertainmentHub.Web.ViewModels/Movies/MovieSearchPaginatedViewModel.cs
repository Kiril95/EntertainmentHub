namespace EntertainmentHub.Web.ViewModels.Movies
{
    public class MovieSearchPaginatedViewModel
    {
        public PaginatedList<MovieSimpleViewModel> Movies { get; set; }

        public int TotalCount { get; set; }

        public int PageResults()
        {
            int pageResults = 0;

            if (this.TotalCount < this.Movies.PageNumber * 24)
            {
                return pageResults = this.TotalCount;
            }
            else
            {
                return pageResults = this.Movies.PageNumber * 24;
            }
        }
    }
}
