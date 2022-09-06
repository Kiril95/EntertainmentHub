namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Linq;

    public interface IGenresService
    {
        IQueryable<T> GetAllGenresAsQueryable<T>();
    }
}
