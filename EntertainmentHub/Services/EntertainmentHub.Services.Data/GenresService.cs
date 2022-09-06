namespace EntertainmentHub.Services.Data
{
    using System.Linq;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;

    public class GenresService : IGenresService
    {
        private readonly IDeletableEntityRepository<Genre> genresRepository;

        public GenresService(IDeletableEntityRepository<Genre> genresRepository)
        {
            this.genresRepository = genresRepository;
        }

        public IQueryable<T> GetAllGenresAsQueryable<T>()
        {
            return this.genresRepository.AllAsNoTracking().To<T>();
        }
    }
}
