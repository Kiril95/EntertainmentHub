namespace EntertainmentHub.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels;
    using EntertainmentHub.Web.ViewModels.Actors;
    using Microsoft.AspNetCore.Mvc;

    public class ActorsController : Controller
    {
        private readonly IActorsService actorsService;

        public ActorsController(IActorsService actorsService)
        {
            this.actorsService = actorsService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await this.actorsService.GetActorByIdAsync<ActorViewModel>(id);

            if (actor is null)
            {
                return this.NotFound();
            }

            return this.View(actor);
        }

        public async Task<IActionResult> All(string searchWord, int page = 1)
        {
            var actors = this.actorsService.GetAllActorsAsQueryable<ActorListViewModel>();

            this.ViewData["CurrentSearchWord"] = searchWord;

            if (!string.IsNullOrEmpty(searchWord))
            {
                actors = actors.Where(x => x.Name.ToLower().Contains(searchWord.ToLower()));
            }

            actors = actors.Where(x => !string.IsNullOrWhiteSpace(x.Name));
            var paginated = await PaginatedList<ActorListViewModel>.CreateAsync(actors, page, 25);

            var paginatedView = new ActorPaginatedListViewModel
            {
                Actors = paginated,
                TotalCount = actors.Count(),
            };

            return this.View(paginatedView);
        }
    }
}
