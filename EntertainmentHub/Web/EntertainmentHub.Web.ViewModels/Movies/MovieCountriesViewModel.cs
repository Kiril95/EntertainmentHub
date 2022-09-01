namespace EntertainmentHub.Web.ViewModels.Movies
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieCountriesViewModel : IMapFrom<MovieCountry>
    {
        public int MovieId { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; }
    }
}
