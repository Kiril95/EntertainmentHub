namespace EntertainmentHub.Services.Data
{
    using System.Linq;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;

    public class SearchService : ISearchService
    {
        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IDeletableEntityRepository<Actor> actorsRrepository;

        public SearchService(
            IDeletableEntityRepository<Movie> moviesRepository,
            IDeletableEntityRepository<Actor> actorsRrepository)
        {
            this.moviesRepository = moviesRepository;
            this.actorsRrepository = actorsRrepository;
        }

        public IQueryable<T> SearchMoviesByTitleAsQueryable<T>(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                return this.moviesRepository
                    .AllAsNoTracking()
                    .Where(x => x.Title.ToLower().Contains(title.ToLower()))
                    .To<T>();
            }

            return null;
        }

        public IQueryable<T> SearchActorsByNameAsQueryable<T>(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return this.actorsRrepository
                    .AllAsNoTracking()
                    .Where(x => x.Name.ToLower().Contains(name.ToLower()))
                    .To<T>();
            }

            return null;
        }
    }
}
