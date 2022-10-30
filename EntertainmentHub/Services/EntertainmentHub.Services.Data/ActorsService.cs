namespace EntertainmentHub.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ActorsService : IActorsService
    {
        private readonly IDeletableEntityRepository<Actor> actorRepository;

        public ActorsService(IDeletableEntityRepository<Actor> actorRepository)
        {
            this.actorRepository = actorRepository;
        }

        public async Task<T> GetActorByIdAsync<T>(int id)
        {
            return await this.actorRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public IQueryable<T> GetAllActorsAsQueryable<T>()
        {
            return this.actorRepository
                .AllAsNoTracking()
                .OrderBy(x => x.Name)
                .To<T>();
        }

        public IQueryable<T> GetMostPopularActorsAsQueryable<T>()
        {
            return this.actorRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.Popularity)
                .To<T>();
        }
    }
}
