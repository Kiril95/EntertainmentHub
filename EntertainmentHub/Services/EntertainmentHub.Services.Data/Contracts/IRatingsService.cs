namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Models;

    public interface IRatingsService
    {
        Task RateAsync(double rate, int movieId, string userId);

        Task RemoveRateAsync(int movieId, string userId);

        Task<double> GetAverageRatingAsync(int movieId);

        Task<Rating> GetRatingAsync(int movieId, string userId);
    }
}
