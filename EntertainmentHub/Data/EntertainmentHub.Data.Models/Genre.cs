namespace EntertainmentHub.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class Genre : BaseDeletableModel<int>
    {
        public Genre()
        {
            this.Movies = new HashSet<MovieGenre>();
        }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        public virtual ICollection<MovieGenre> Movies { get; set; }
    }
}
