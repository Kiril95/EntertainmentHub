namespace EntertainmentHub.Web.ViewModels.Actors
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class ActorViewModel : IMapFrom<Actor>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Birthplace { get; set; }

        public string Gender { get; set; }

        public string Biography { get; set; }

        public string Photo { get; set; }

        public double Popularity { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public int? Age
        {
            get
            {
                DateTime today = DateTime.Today;

                if (this.DateOfDeath.HasValue && this.DateOfBirth.HasValue)
                {
                    return this.DateOfDeath.Value.Year - this.DateOfBirth.Value.Year;
                }
                else if (this.DateOfBirth.HasValue)
                {
                    return today.Year - this.DateOfBirth.Value.Year;
                }

                return null;
            }
        }

        public ICollection<MovieActorsViewModel> Movies { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Actor, ActorViewModel>()
                .ForMember(x => x.Photo, opt => opt.MapFrom(x => x.Photo.Contains("jpg") ? x.Photo : "/images/no-actor-photo.jpg"));
        }
    }
}
