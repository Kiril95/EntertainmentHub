namespace EntertainmentHub.Web.Controllers
{
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Contact;
    using Microsoft.AspNetCore.Mvc;

    public class ContactController : Controller
    {
        private readonly IContactService contactsService;

        public ContactController(IContactService contactsService)
        {
            this.contactsService = contactsService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactFormInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.contactsService.GetUserSubmissionAsync(inputModel);

            return this.RedirectToAction(nameof(this.AcceptedSubmission));
        }

        public IActionResult AcceptedSubmission()
        {
            return this.View();
        }
    }
}
