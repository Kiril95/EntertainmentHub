namespace EntertainmentHub.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using EntertainmentHub.Web.ViewModels.Administration.Collector;
    using Microsoft.AspNetCore.Mvc;

    public class CollectorController : AdministrationController
    {
        public IActionResult CollectData()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CollectData(GetDataInputModel inputModel)
        {
            // TODO
            if (this.User.IsInRole("BasicUser"))
            {
                await Task.FromResult(0);
            }

            return this.RedirectToAction(nameof(this.Success));
        }

        public IActionResult Success()
        {
            return this.View();
        }
    }
}
