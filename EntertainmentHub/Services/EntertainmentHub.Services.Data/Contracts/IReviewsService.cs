namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IReviewsService
    {
        Task<T> GetReviewByIdAsync<T>(int id);

        IQueryable<T> GetReviewsByIdAsQueryable<T>(int id);
    }
}
