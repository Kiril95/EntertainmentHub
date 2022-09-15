namespace EntertainmentHub.Web.Controllers
{
    using System.Diagnostics;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels;
    using EntertainmentHub.Web.ViewModels.Movies;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IMoviesService moviesService;

        public HomeController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public IActionResult Index()
        {
            var recentMovies = this.moviesService.GetRecentMoviesAsQueryable<MovieHomeViewModel>();
            var popularMovies = this.moviesService.GetPopularMoviesAsQueryable<MovieHomeViewModel>();
            var topMovies = this.moviesService.GetTopRatedMoviesAsQueryable<MovieHomeViewModel>();
            var latestMovies = this.moviesService.GetLatestMoviesAsQueryable<MovieSimpleViewModel>();

            var viewModel = new HomepageViewModel
            {
                RecentMovies = recentMovies,
                PopularMovies = popularMovies,
                TopRatedMovies = topMovies,
                LatestMovies = latestMovies,
            };

            return this.View(viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult CookiePolicy()
        {
            return this.View();
        }

        public IActionResult Credits()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [Route("/Home/ErrorView/{status:int}")]
        public IActionResult ErrorView(int status)
        {
            return this.View("~/Views/Shared/Error404.cshtml");
        }
    }
}
