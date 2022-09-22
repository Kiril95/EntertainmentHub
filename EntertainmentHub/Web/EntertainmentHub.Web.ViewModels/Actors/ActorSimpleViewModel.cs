namespace EntertainmentHub.Web.ViewModels.Actors
{
    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class ActorSimpleViewModel : IMapFrom<Actor>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Photo { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Actor, ActorSimpleViewModel>()
                .ForMember(x => x.Photo, opt => opt.MapFrom(x => x.Photo.Contains("jpg") ? x.Photo : "/images/no-actor-photo.jpg"));
        }
    }
}
