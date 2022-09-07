namespace EntertainmentHub.Services.Data
{
    using System.Linq;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;

    public class CountriesService : ICountriesService
    {
        private readonly IDeletableEntityRepository<Country> countriesRepository;

        public CountriesService(IDeletableEntityRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        public IQueryable<T> GetAllCountriesAsQueryable<T>()
        {
            return this.countriesRepository.AllAsNoTracking().To<T>();
        }
    }
}
