namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System;
    using System.Linq;

    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieSimpleViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Poster { get; set; }

        public DateTime ReleaseDate { get; set; }

        public double Rating { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, MovieSimpleViewModel>()
                .ForMember(x => x.Rating, opt => opt.MapFrom(x => x.Ratings.Count > 0 ? x.Ratings.Average(x => x.Rate) : 0));
        }
    }
}
