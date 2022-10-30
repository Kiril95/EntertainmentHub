namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IActorsService
    {
        Task<T> GetActorByIdAsync<T>(int id);

        IQueryable<T> GetAllActorsAsQueryable<T>();

        IQueryable<T> GetMostPopularActorsAsQueryable<T>();
    }
}
