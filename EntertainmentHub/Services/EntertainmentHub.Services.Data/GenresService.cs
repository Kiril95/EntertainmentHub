namespace EntertainmentHub.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<T>> GetMainGenresAsync<T>()
        {
            return await this.genresRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.Movies.Count())
                .Take(14)
                .To<T>()
                .ToListAsync();
        }
    }
}
