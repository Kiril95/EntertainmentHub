namespace EntertainmentHub.Web.ViewModels.Movies
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieLanguageViewModel : IMapFrom<MovieLanguage>
    {
        public int MovieId { get; set; }

        public int LanguageId { get; set; }

        public string LanguageName { get; set; }
    }
}
