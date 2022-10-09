namespace EntertainmentHub.Web.Areas.Administration.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EntertainmentHub.Common;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Contact;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ContactController : AdministrationController
    {
        private readonly IContactService contactService;
        private readonly UserManager<ApplicationUser> userManager;

        public ContactController(
            IContactService contactsService,
            UserManager<ApplicationUser> userManager)
        {
            this.contactService = contactsService;
            this.userManager = userManager;
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

        public async Task<IActionResult> Reply(int id)
        {
            var submission = await this.contactService.GetSubmissionByIdAsync<ContactViewModel>(id);
            var user = await this.userManager.GetUserAsync(this.User);

            var viewModel = new ReplyModel
            {
                Id = submission.Id,
                Name = $"{GlobalConstants.SystemName} - {user.UserName}",
                Email = user.Email,
                To = submission.Email,
                Subject = $"RE: {submission.Subject}",
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(ReplyModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.contactService.ReplyToUserAsync(inputModel);

            return this.RedirectToAction(nameof(this.AllSubmissions), new { area = "Administration" });
        }
    }
}
