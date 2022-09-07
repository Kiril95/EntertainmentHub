namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System;
    using System.Linq;

    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieListViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Poster { get; set; }

        public string Director { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int Runtime { get; set; }

        public double Budget { get; set; }

        public double Revenue { get; set; }

        public double Popularity { get; set; }

        public string Languages { get; set; }

        public string Genres { get; set; }

        public string Countries { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, MovieListViewModel>()
                .ForMember(x => x.Languages, opt => opt.MapFrom(x => string.Join(", ", x.Languages.Select(l => l.Language.Name))))
                .ForMember(x => x.Genres, opt => opt.MapFrom(x => string.Join(", ", x.MovieGenres.Select(l => l.Genre.Name))))
                .ForMember(x => x.Countries, opt => opt.MapFrom(x => string.Join(", ", x.MovieCountries.Select(l => l.Country.Name))));
        }
    }
}
