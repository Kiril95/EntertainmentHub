namespace EntertainmentHub.Web.Controllers
{
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Movies;
    using EntertainmentHub.Web.ViewModels.Reviews;
    using Microsoft.AspNetCore.Mvc;

    public class ReviewsController : Controller
    {
        private readonly IReviewsService reviewsService;
        private readonly IMoviesService moviesService;

        public ReviewsController(
            IReviewsService reviewsService,
            IMoviesService moviesService)
        {
            this.reviewsService = reviewsService;
            this.moviesService = moviesService;
        }

        public async Task<IActionResult> Details(int id, int movieId)
        {
            var movie = await this.moviesService.GetMovieByIdAsync<MovieSimpleViewModel>(movieId);
            var review = await this.reviewsService.GetReviewByIdAsync<MovieReviewViewModel>(id);

            var viewModel = new MovieReviewPageViewModel
            {
                MovieModel = movie,
                ReviewModel = review,
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> All(int id)
        {
            var movie = await this.moviesService.GetMovieByIdAsync<MovieReviewsCollectionViewModel>(id);

            return this.View(movie);
        }
    }
}
