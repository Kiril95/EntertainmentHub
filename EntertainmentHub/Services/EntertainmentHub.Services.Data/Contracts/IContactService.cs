namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using EntertainmentHub.Web.ViewModels.Contact;

    public interface IContactService
    {
        Task GetUserSubmitionAsync(ContactFormInputModel inputModel);

        Task DeleteUserSubmitionAsync(int id);
    }
}
