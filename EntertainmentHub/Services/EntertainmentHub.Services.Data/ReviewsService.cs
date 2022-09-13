namespace EntertainmentHub.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ReviewsService : IReviewsService
    {
        private readonly IRepository<MovieReview> movieReviewsRepository;

        public ReviewsService(IRepository<MovieReview> movieReviewsRepository)
        {
            this.movieReviewsRepository = movieReviewsRepository;
        }

        public async Task<T> GetReviewByIdAsync<T>(int id)
        {
            return await this.movieReviewsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public IQueryable<T> GetReviewsByIdAsQueryable<T>(int id)
        {
            return this.movieReviewsRepository
                .AllAsNoTracking()
                .Where(x => x.MovieId == id)
                .To<T>();
        }
    }
}
