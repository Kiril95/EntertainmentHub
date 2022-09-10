namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IReviewsService
    {
        Task<T> GetReviewByIdAsync<T>(int id);
    }
}
