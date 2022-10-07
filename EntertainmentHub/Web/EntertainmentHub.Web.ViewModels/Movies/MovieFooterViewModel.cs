namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System.Linq;

    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieFooterViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string SlideshowPath { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, MovieFooterViewModel>()
                .ForMember(x => x.SlideshowPath, opt => opt.MapFrom(x => x.Slideshow.FirstOrDefault().Path));
        }
    }
}
