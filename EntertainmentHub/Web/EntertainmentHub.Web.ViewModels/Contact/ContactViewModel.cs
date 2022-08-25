namespace EntertainmentHub.Web.ViewModels.Contact
{
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class ContactViewModel : IMapFrom<ContactForm>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
