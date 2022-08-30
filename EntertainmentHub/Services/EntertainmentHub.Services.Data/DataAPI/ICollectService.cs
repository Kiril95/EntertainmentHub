namespace EntertainmentHub.Services.Data.DataAPI
{
    using System.Threading.Tasks;

    public interface ICollectService
    {
        Task<int> AddMoviesToDatabaseAsync(int startIndex, int endIndex);

        Task AddTVShowsToDatabaseAsync(int startIndex, int endIndex);
    }
}
