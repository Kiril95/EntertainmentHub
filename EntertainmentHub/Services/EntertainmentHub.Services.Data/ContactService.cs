namespace EntertainmentHub.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Services.Messaging;
    using EntertainmentHub.Web.ViewModels.Contact;
    using Microsoft.EntityFrameworkCore;

    public class ContactService : IContactService
    {
        private readonly IRepository<ContactForm> contactsRepository;
        private readonly IEmailSender emailSender;

        public ContactService(
            IRepository<ContactForm> userContactsRepository,
            IEmailSender emailSender)
        {
            this.contactsRepository = userContactsRepository;
            this.emailSender = emailSender;
        }

        public async Task GetUserSubmissionAsync(ContactFormInputModel inputModel)
        {
            var query = new ContactForm
            {
                Name = inputModel.Name,
                Email = inputModel.Email,
                Subject = inputModel.Subject,
                Message = inputModel.Message,
            };

            await this.contactsRepository.AddAsync(query);

            await this.contactsRepository.SaveChangesAsync();
        }

        public IQueryable<T> GetSubmissionsAsQueryable<T>()
        {
            return this.contactsRepository.AllAsNoTracking().To<T>();
        }

        public async Task<T> GetSubmissionByIdAsync<T>(int id)
        {
            return await this.contactsRepository
                 .AllAsNoTracking()
                 .Where(x => x.Id == id)
                 .To<T>()
                 .FirstOrDefaultAsync();
        }

        public async Task DeleteSubmissionAsync(int id)
        {
            var submission = await this.contactsRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            this.contactsRepository.Delete(submission);

            await this.contactsRepository.SaveChangesAsync();
        }

        public async Task ReplyToUserAsync(ReplyModel inputModel)
        {
            await this.emailSender.SendEmailAsync(
                inputModel.Email,
                inputModel.Name,
                inputModel.To,
                inputModel.Subject,
                inputModel.Message);
        }
    }
}
