namespace EntertainmentHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class Review : BaseModel<int>
    {
        public Review()
        {
            this.MovieReviews = new HashSet<MovieReview>();
        }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 2)]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<MovieReview> MovieReviews { get; set; }
    }
}
