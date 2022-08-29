namespace EntertainmentHub.Data.Models
{
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
        public string AuthorName { get; set; }

        [Required]
        public string AuthorUsername { get; set; }

        public string AvatarPath { get; set; }

        [Required]
        public string Content { get; set; }

        public virtual ICollection<MovieReview> MovieReviews { get; set; }
    }
}
