namespace EntertainmentHub.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class MovieReview : BaseModel<int>
    {
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        [Required]
        public string AuthorName { get; set; }

        [Required]
        public string AuthorUsername { get; set; }

        public string AvatarPath { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
