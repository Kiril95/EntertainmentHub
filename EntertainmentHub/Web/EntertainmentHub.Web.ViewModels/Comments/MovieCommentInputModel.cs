namespace EntertainmentHub.Web.ViewModels.Comments
{
    using System.ComponentModel.DataAnnotations;

    public class MovieCommentInputModel
    {
        public int MovieId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Content { get; set; }
    }
}
