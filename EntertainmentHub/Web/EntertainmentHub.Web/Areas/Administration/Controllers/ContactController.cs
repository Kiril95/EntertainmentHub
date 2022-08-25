namespace EntertainmentHub.Web.Areas.Administration.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Contact;
    using Microsoft.AspNetCore.Mvc;

    public class ContactController : AdministrationController
    {
        private readonly IContactService contactService;

        public ContactController(
            IContactService contactsService)
        {
            this.contactService = contactsService;
        }

        public IActionResult AllSubmissions()
        {
            var enquiries = this.contactService.GetSubmissionsAsQueryable<ContactViewModel>();

            return this.View(enquiries);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await this.contactService.GetSubmissionByIdAsync<ContactViewModel>(id);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.contactService.DeleteSubmissionAsync(id);

            return this.RedirectToAction(nameof(this.AllSubmissions), "Contact");
        }
    }
}
