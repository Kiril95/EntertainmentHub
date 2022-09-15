namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System.Collections.Generic;

    public class HomepageViewModel
    {
        public IEnumerable<MovieHomeViewModel> RecentMovies { get; set; }

        public IEnumerable<MovieHomeViewModel> PopularMovies { get; set; }

        public IEnumerable<MovieHomeViewModel> TopRatedMovies { get; set; }

        public IEnumerable<MovieSimpleViewModel> LatestMovies { get; set; }
    }
}
