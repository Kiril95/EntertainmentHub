namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieHomeViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Poster { get; set; }

        public string Trailer { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Description { get; set; }

        public double Rating { get; set; }

        public ICollection<MovieGenresViewModel> MovieGenres { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, MovieHomeViewModel>()
                .ForMember(x => x.Rating, opt => opt.MapFrom(x => x.Ratings.Count > 0 ? x.Ratings.Average(x => x.Rate) : 0));
        }
    }
}
