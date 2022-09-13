namespace EntertainmentHub.Web.ViewModels.Movies
{
    public class MoviePaginatedListViewModel
    {
        public PaginatedList<MovieListViewModel> Movies { get; set; }

        public int TotalCount { get; set; }

        public int PageResults()
        {
            int pageResults = 0;

            if (this.TotalCount < this.Movies.PageNumber * 20)
            {
                return pageResults = this.TotalCount;
            }
            else
            {
                return pageResults = this.Movies.PageNumber * 20;
            }
        }
    }
}
