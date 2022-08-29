namespace EntertainmentHub.Services.Data.DataAPI
{
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.DataAPI.DataModels;

    public interface IDataService
    {
        Task<MovieDTO> GetMovieDataAsync(int movieId);

        Task<CastAndCrewDTO> GetCastAndCrewAsync(int movieId);

        Task<ActorDTO> GetActorAsync(int actorId);

        Task<TrailerDTO> GetMovieTrailersAsync(int movieId);

        Task<SlideshowDTO> GetMoviePhotoSlidesAsync(int movieId);

        Task<MovieReviewDTO> GetMovieReviewAsync(int movieId);
    }
}
