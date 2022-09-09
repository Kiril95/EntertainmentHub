namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Web.ViewModels.Comments;

    public class MovieViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Tagline { get; set; }

        public string Poster { get; set; }

        public string Trailer { get; set; }

        public string Director { get; set; }

        public string IMDBLink { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int Runtime { get; set; }

        public double Budget { get; set; }

        public double Revenue { get; set; }

        public double Popularity { get; set; }

        public string Status { get; set; }

        public double Rating { get; set; }

        public string Languages { get; set; }

        public string Genres { get; set; }

        public string Countries { get; set; }

        public ICollection<MovieCastViewModel> MovieActors { get; set; }

        public ICollection<MovieReviewViewModel> MovieReviews { get; set; }

        public ICollection<MovieCommentViewModel> MovieComments { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, MovieViewModel>()
                .ForMember(x => x.Languages, opt => opt.MapFrom(x => string.Join(", ", x.Languages.Select(l => l.Language.Name))))
                .ForMember(x => x.Genres, opt => opt.MapFrom(x => string.Join(", ", x.MovieGenres.Select(l => l.Genre.Name))))
                .ForMember(x => x.Countries, opt => opt.MapFrom(x => string.Join(", ", x.MovieCountries.Select(l => l.Country.Name))))
                .ForMember(x => x.Rating, opt => opt.MapFrom(x => x.Ratings.Count > 0 ? x.Ratings.Average(x => x.Rate) : 0));
        }
    }
}
