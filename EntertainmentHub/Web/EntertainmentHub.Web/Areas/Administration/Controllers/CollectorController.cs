namespace EntertainmentHub.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.DataAPI;
    using EntertainmentHub.Web.ViewModels.Administration.Collector;
    using Microsoft.AspNetCore.Mvc;

    public class CollectorController : AdministrationController
    {
        private readonly ICollectService collectService;

        public CollectorController(ICollectService collectService)
        {
            this.collectService = collectService;
        }

        public IActionResult CollectData()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CollectData(GetDataInputModel inputModel)
        {
            if (inputModel.StartIndex > inputModel.EndIndex)
            {
                this.ModelState.AddModelError(string.Empty, "End index cannot be less than Start index !");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.collectService.AddMoviesToDatabaseAsync(inputModel.StartIndex, inputModel.EndIndex);

            return this.RedirectToAction(nameof(this.Success));
        }

        public IActionResult Success()
        {
            return this.View();
        }
    }
}
