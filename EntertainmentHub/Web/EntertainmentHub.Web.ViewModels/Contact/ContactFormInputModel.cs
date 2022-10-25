namespace EntertainmentHub.Web.ViewModels.Contact
{
    using System.ComponentModel.DataAnnotations;

    using PressCenters.Web.Infrastructure;

    public class ContactFormInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} should be between {2} and {1} characters!")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "{0} should be between {2} and {1} characters!")]
        public string Subject { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 4, ErrorMessage = "{0} should be between {2} and {1} characters!")]
        public string Message { get; set; }

        [ReCaptchaValidationAttribute]
        public string ReCaptchaValue { get; set; }
    }
}
