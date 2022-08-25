namespace EntertainmentHub.Services.Data
{
    using System;
    using System.Collections.Generic;
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

        public ContactService(
            IRepository<ContactForm> userContactsRepository)
        {
            this.contactsRepository = userContactsRepository;
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
    }
}
