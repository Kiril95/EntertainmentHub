namespace EntertainmentHub.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class MovieSlide : BaseDeletableModel<int>
    {
        [Required]
        [StringLength(150, MinimumLength = 15)]
        public string Path { get; set; }

        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }
    }
}
