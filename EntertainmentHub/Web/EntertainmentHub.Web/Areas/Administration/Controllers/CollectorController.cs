namespace EntertainmentHub.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.DataAPI;
    using EntertainmentHub.Web.ViewModels.Administration.Collector;
    using Microsoft.AspNetCore.Mvc;

    public class CollectorController : AdministrationController
    {
        private readonly ICollectService collectService;
        private readonly IDeletableEntityRepository<Movie> moviesRepository;

        public CollectorController(
            ICollectService collectService,
            IDeletableEntityRepository<Movie> moviesRepository)
        {
            this.collectService = collectService;
            this.moviesRepository = moviesRepository;
        }

        public IActionResult CollectData()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CollectData(GetDataInputModel inputModel)
        {
            // It could be done in a better way :/
            if (inputModel.StartIndex > inputModel.EndIndex)
            {
                this.ModelState.AddModelError(string.Empty, "End index cannot be less than Start index !");
                return this.View(inputModel);
            }

            for (int i = inputModel.StartIndex; i <= inputModel.EndIndex; i++)
            {
                var currentMovie = this.moviesRepository.AllAsNoTracking().FirstOrDefault(x => x.TMDBId == i);

                if (currentMovie is not null)
                {
                    this.ModelState.AddModelError(string.Empty, $"Movie with Id: ({i}) already exists !");
                }
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            var movies = await this.collectService.AddMoviesToDatabaseAsync(inputModel.StartIndex, inputModel.EndIndex);

            return this.RedirectToAction(nameof(this.Success), new { count = movies });
        }

        public IActionResult Success(int count)
        {
            this.ViewData["count"] = count;
            return this.View(count);
        }
    }
}
