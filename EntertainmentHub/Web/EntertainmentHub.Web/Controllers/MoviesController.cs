namespace EntertainmentHub.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Movies;
    using Microsoft.AspNetCore.Mvc;

    public class MoviesController : Controller
    {
        private readonly IMoviesService moviesService;

        public MoviesController(
            IMoviesService moviesService)
        {
            this.moviesService = moviesService;
        }

        public IActionResult Collection(string searchWord)
        {
            var movies = this.moviesService.GetAllMoviesAsQueryable<MovieListViewModel>();

            this.ViewData["CurrentSearchWord"] = searchWord;

            if (!string.IsNullOrEmpty(searchWord))
            {
                movies = movies.Where(x => x.Title.ToLower().Contains(searchWord.ToLower()));
            }

            return this.View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await this.moviesService.GetMovieByIdAsync<MovieViewModel>(id);

            return this.View(movie);
        }
    }
}
