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

        public Gender Gender { get; set; }

        [MaxLength(50)]
        public string Birthplace { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        [Required]
        public string Biography { get; set; }

        [MaxLength(300)]
        public string Photo { get; set; }

        public double Popularity { get; set; }

        public virtual ICollection<MovieActor> Movies { get; set; }
    }
}
