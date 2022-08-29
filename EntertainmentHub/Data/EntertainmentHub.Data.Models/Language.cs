namespace EntertainmentHub.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class Language : BaseDeletableModel<int>
    {
        public Language()
        {
            this.Movies = new HashSet<MovieLanguage>();
        }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Name { get; set; }

        public virtual ICollection<MovieLanguage> Movies { get; set; }
    }
}
