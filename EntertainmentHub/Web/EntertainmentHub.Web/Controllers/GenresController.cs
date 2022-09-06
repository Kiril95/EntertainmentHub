namespace EntertainmentHub.Web.Controllers
{
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Movies;
    using Microsoft.AspNetCore.Mvc;

    public class GenresController : Controller
    {
        private readonly IMoviesService moviesService;

        public GenresController(IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public IActionResult Type(string name)
        {
            var movies = this.moviesService.GetMoviesByGenreAsQueryable<MovieSimpleViewModel>(name);

            return this.View(movies);
        }
    }
}
