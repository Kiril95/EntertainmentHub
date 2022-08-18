namespace EntertainmentHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class MovieActor : IDeletableEntity
    {
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int ActorId { get; set; }

        public virtual Actor Actor { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string CharacterPlayed { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
