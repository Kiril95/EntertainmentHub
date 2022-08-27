namespace EntertainmentHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;
    using EntertainmentHub.Data.Models.Enumerations;

    public class Actor : BaseDeletableModel<int>
    {
        public Actor()
        {
            this.Movies = new HashSet<MovieActor>();
        }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Birthplace { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        [Required]
        public string Biography { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 20)]
        public string Photo { get; set; }

        public double Popularity { get; set; }

        public virtual ICollection<MovieActor> Movies { get; set; }
    }
}
