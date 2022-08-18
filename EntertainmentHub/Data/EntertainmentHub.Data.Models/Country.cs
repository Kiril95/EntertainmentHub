namespace EntertainmentHub.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class Country : BaseDeletableModel<int>
    {
        public Country()
        {
            this.MovieCountries = new HashSet<MovieCountry>();
        }

        [Required]
        [StringLength(40, MinimumLength = 3)]
        public string Name { get; set; }

        public ICollection<MovieCountry> MovieCountries { get; set; }
    }
}
