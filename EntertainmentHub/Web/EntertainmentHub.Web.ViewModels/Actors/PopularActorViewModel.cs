namespace EntertainmentHub.Web.ViewModels.Actors
{
    using System;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Data.Models.Enumerations;
    using EntertainmentHub.Services.Mapping;

    public class PopularActorViewModel : IMapFrom<Actor>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Birthplace { get; set; }

        public Gender Gender { get; set; }

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
    }
}
