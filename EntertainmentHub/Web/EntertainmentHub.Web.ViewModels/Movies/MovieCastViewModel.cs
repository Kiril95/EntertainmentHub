namespace EntertainmentHub.Web.ViewModels.Movies
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieCastViewModel : IMapFrom<MovieActor>
    {
        public int ActorId { get; set; }

        public string ActorName { get; set; }

        public string CharacterName { get; set; }

        public string ActorPhoto { get; set; }
    }
}
