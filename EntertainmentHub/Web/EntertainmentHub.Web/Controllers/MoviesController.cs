namespace EntertainmentHub.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels;
    using EntertainmentHub.Web.ViewModels.Comments;
    using EntertainmentHub.Web.ViewModels.Movies;
    using EntertainmentHub.Web.ViewModels.Reviews;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class MoviesController : Controller
    {
        private readonly IMoviesService moviesService;
        private readonly ICommentsService commentsService;

        public MoviesController(
            IMoviesService moviesService,
            ICommentsService commentsService)
        {
            this.moviesService = moviesService;
            this.commentsService = commentsService;
        }

        public async Task<IActionResult> Collection(string searchWord, int page = 1)
        {
            var movies = this.moviesService.GetAllMoviesAsQueryable<MovieListViewModel>();

            this.ViewData["CurrentSearchWord"] = searchWord;

            if (!string.IsNullOrEmpty(searchWord))
            {
                movies = movies.Where(x => x.Title.ToLower().Contains(searchWord.ToLower()));
            }

            var paginated = await PaginatedList<MovieListViewModel>.CreateAsync(movies, page, 20);

            var viewModel = new MoviePaginatedListViewModel
            {
                Movies = paginated,
                TotalCount = movies.Count(),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Details(int id, int page = 1)
        {
            var movie = await this.moviesService.GetMovieByIdAsync<MovieViewModel>(id);
            var comments = this.commentsService.GetCommentsByIdAsQueryable<MovieCommentViewModel>(id);

            if (movie is null)
            {
                return this.NotFound();
            }

            var paginated = await PaginatedList<MovieCommentViewModel>.CreateAsync(comments, page, 5);

            var viewModel = new MovieDetailsPaginatedViewModel
            {
                Movie = movie,
                Comments = paginated,
                TotalCount = comments.Count(),
            };

            return this.View(viewModel);
        }
    }
}
