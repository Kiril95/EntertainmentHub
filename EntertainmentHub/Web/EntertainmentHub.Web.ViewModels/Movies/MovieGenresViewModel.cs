namespace EntertainmentHub.Web.ViewModels.Movies
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieGenresViewModel : IMapFrom<MovieGenre>
    {
        public int MovieId { get; set; }

        public int GenreId { get; set; }

        public string GenreName { get; set; }
    }
}
