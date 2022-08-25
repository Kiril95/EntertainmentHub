namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Web.ViewModels.Contact;

    public interface IContactService
    {
        Task GetUserSubmissionAsync(ContactFormInputModel inputModel);

        Task DeleteSubmissionAsync(int id);

        IQueryable<T> GetSubmissionsAsQueryable<T>();

        Task<T> GetSubmissionByIdAsync<T>(int id);
    }
}
