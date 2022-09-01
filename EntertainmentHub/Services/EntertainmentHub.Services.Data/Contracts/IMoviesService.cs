namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Web.ViewModels.Administration.Movies;
    using EntertainmentHub.Web.ViewModels.Movies;

    public interface IMoviesService
    {
        Task CreateAsync(CreateMovieInputModel inputModel);

        Task DeleteAsync(int id);

        IQueryable<T> GetAllMoviesAsQueryable<T>();

        Task<T> GetRandomMovieForBannerAsync<T>();
    }
}
