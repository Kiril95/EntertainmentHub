namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IGenresService
    {
        IQueryable<T> GetAllGenresAsQueryable<T>();

        Task<IEnumerable<T>> GetMainGenresAsync<T>();
    }
}
