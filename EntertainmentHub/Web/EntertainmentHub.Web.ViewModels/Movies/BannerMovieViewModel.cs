namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System.Linq;

    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class BannerMovieViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Tagline { get; set; }

        public string Slideshow { get; set; }

        public string Trailer { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            // Get all the photos ready here, by correctly referencing every image
            configuration.CreateMap<Movie, BannerMovieViewModel>()
                .ForMember(m => m.Slideshow, opt => opt.MapFrom(x => string.Join(", ", x.Slideshow.Select(x => "\"" + x.Path + "\""))));
        }
    }
}
