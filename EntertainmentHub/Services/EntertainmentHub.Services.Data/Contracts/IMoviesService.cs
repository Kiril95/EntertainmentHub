namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using EntertainmentHub.Web.ViewModels.Administration.Movies;

    public interface IMoviesService
    {
        Task CreateAsync(CreateMovieInputModel inputModel);

        Task DeleteAsync(int id);
    }
}
