namespace EntertainmentHub.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Messaging;
    using EntertainmentHub.Web.ViewModels.Contact;
    using Microsoft.EntityFrameworkCore;

    public class ContactService : IContactService
    {
        private readonly IRepository<ContactForm> contactsRepository;

        public ContactService(
            IRepository<ContactForm> userContactsRepository)
        {
            this.contactsRepository = userContactsRepository;
        }

        public async Task GetUserSubmitionAsync(ContactFormInputModel inputModel)
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

        public async Task DeleteUserSubmitionAsync(int id)
        {
            var enquiry = await this.contactsRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (enquiry == null)
            {
                throw new NullReferenceException($"Enquiry with id {id} does not exists.");
            }

            this.contactsRepository.Delete(enquiry);
            await this.contactsRepository.SaveChangesAsync();
        }
    }
}
