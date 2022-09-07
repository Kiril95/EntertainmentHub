namespace EntertainmentHub.Web.Controllers
{
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Movies;
    using Microsoft.AspNetCore.Mvc;

    public class CountriesController : Controller
    {
        private readonly IMoviesService moviesService;

        public CountriesController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public IActionResult Type(string name)
        {
            var movies = this.moviesService.GetMoviesByCountryAsQueryable<MovieSimpleViewModel>(name);

            return this.View(movies);
        }
    }
}
