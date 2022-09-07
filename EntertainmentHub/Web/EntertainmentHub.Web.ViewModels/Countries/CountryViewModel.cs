namespace EntertainmentHub.Web.ViewModels.Countries
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class CountryViewModel : IMapFrom<Country>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
