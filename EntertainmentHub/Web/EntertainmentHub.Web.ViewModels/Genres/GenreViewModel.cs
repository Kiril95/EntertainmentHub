namespace EntertainmentHub.Web.ViewModels.Genres
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class GenreViewModel : IMapFrom<Genre>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
