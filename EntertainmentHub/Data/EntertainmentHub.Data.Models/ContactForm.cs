namespace EntertainmentHub.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class ContactForm : BaseModel<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Subject { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 4)]
        public string Message { get; set; }
    }
}
