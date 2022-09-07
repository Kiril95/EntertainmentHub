namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Linq;

    public interface ICountriesService
    {
        IQueryable<T> GetAllCountriesAsQueryable<T>();
    }
}
