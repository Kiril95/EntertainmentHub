namespace EntertainmentHub.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Rating;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingsService ratingsService;

        public RatingsController(IRatingsService ratingsService)
        {
            this.ratingsService = ratingsService;
        }

        [Authorize]
        public async Task Post(RatingInputModel input)
        {
            // Pass the data with a model because the api call needs one with the same names
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.ratingsService.RateAsync(input.Rate, input.MovieId, userId);
        }

        [Authorize]
        [HttpGet]
        public async Task<double> Get(int id)
        {
            return await this.ratingsService.GetAverageRatingAsync(id);
        }

        [Authorize]
        [HttpDelete]
        public async Task<bool> Delete(DeleteRatingInputModel input)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var rating = await this.ratingsService.GetRatingAsync(input.MovieId, userId);

            if (rating is not null)
            {
                await this.ratingsService.RemoveRateAsync(input.MovieId, userId);

                return true;
            }

            return false;
        }
    }
}
