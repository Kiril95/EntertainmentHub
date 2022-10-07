namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Web.ViewModels.Administration.Movies;

    public interface IMoviesService
    {
        Task CreateAsync(CreateMovieInputModel inputModel);

        Task DeleteAsync(int id);

        IQueryable<T> GetAllMoviesAsQueryable<T>();

        Task<T> GetRandomMovieForBannerAsync<T>();

        IQueryable<T> GetMoviesByGenreAsQueryable<T>(string name);

        IQueryable<T> GetMoviesByCountryAsQueryable<T>(string name);

        Task<T> GetMovieByIdAsync<T>(int id);

        Task<IEnumerable<T>> GetRecentMoviesAsync<T>();

        Task<IEnumerable<T>> GetPopularMoviesAsync<T>();

        IQueryable<T> GetTopRatedMoviesAsQueryable<T>();

        Task<IEnumerable<T>> GetLatestMoviesAsync<T>();

        IQueryable<T> GetMoviesByYearAsQueryable<T>(int year);
    }
}
