namespace EntertainmentHub.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels;
    using EntertainmentHub.Web.ViewModels.Actors;
    using EntertainmentHub.Web.ViewModels.Movies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class SearchController : Controller
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        public async Task<IActionResult> MoviesByTitle(string title, int page = 1)
        {
            var movies = this.searchService.SearchMoviesByTitleAsQueryable<MovieSimpleViewModel>(title);
            var actors = this.searchService.SearchActorsByNameAsQueryable<ActorSimpleViewModel>(title);

            this.ViewData["CurrentSearchWord"] = title;

            if (movies is null || (movies.Count() == 0 && actors.Count() == 0))
            {
                return this.RedirectToAction(nameof(this.NoResults), new { searchWord = title });
            }

            var paginated = await PaginatedList<MovieSimpleViewModel>.CreateAsync(movies, page, 24);

            var viewModel = new MovieSearchPaginatedViewModel
            {
                Movies = paginated,
                TotalCount = movies.Count(),
            };

            this.ViewData["MoviesCount"] = movies.Count();
            this.ViewData["ActorsCount"] = actors.Count();

            return this.View(viewModel);
        }

        public async Task<IActionResult> ActorsByNameAsync(string name, int page = 1)
        {
            var actors = this.searchService.SearchActorsByNameAsQueryable<ActorSimpleViewModel>(name);
            var movies = this.searchService.SearchMoviesByTitleAsQueryable<MovieSimpleViewModel>(name);

            this.ViewData["CurrentSearchWord"] = name;

            if (actors is null || (actors.Count() == 0 && movies.Count() == 0))
            {
                return this.RedirectToAction(nameof(this.NoResults), new { searchWord = name });
            }

            var paginated = await PaginatedList<ActorSimpleViewModel>.CreateAsync(actors, page, 30);

            var viewModel = new ActorSearchPaginatedViewModel
            {
               Actors = paginated,
               TotalCount = actors.Count(),
            };

            this.ViewData["ActorsCount"] = actors.Count();
            this.ViewData["MoviesCount"] = movies.Count();

            return this.View(viewModel);
        }

        public IActionResult NoResults(string searchWord)
        {
            this.ViewData["CurrentSearchWord"] = searchWord;

            return this.View();
        }
    }
}
