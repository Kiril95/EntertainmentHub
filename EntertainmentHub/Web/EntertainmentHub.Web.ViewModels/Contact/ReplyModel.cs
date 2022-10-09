namespace EntertainmentHub.Web.ViewModels.Contact
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ReplyModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} should be between {2} and {1} characters!")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        public string To { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "{0} should be between {2} and {1} characters!")]
        public string Subject { get; set; }

        [Required]
        [DisplayName("Answer")]
        [StringLength(500, MinimumLength = 4, ErrorMessage = "{0} should be between {2} and {1} characters!")]
        public string Message { get; set; }
    }
}
