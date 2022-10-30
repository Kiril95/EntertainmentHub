namespace EntertainmentHub.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EntertainmentHub.Data.Models.Enumerations;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels;
    using EntertainmentHub.Web.ViewModels.Actors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> MostPopular(string male, string female, string ageAsc, string ageDesc,int page = 1)
        {
            var actors = this.actorsService.GetMostPopularActorsAsQueryable<PopularActorViewModel>();

            this.ViewData["IsMaleChecked"] = male;
            this.ViewData["IsFemaleChecked"] = female;
            this.ViewData["IsAgeAscChecked"] = ageAsc;
            this.ViewData["IsAgeDescChecked"] = ageDesc;

            if (male == "on" && female == "on")  { }
            else
            {
                if (male == "on")
                {
                    actors = actors.Where(x => (int)x.Gender == 2);
                }

                if (female == "on")
                {
                    actors = actors.Where(x => (int)x.Gender == 1);
                }
            }

            if (ageAsc == "on")
            {
                actors = actors.Where(x => x.DateOfBirth.HasValue && !x.DateOfDeath.HasValue).OrderByDescending(x => x.DateOfBirth);
            }

            if (ageDesc == "on")
            {
                actors = actors.Where(x => x.DateOfBirth.HasValue && !x.DateOfDeath.HasValue).OrderBy(x => x.DateOfBirth);
            }

            actors = actors.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.Photo.Contains("jpg"));
            var paginated = await PaginatedList<PopularActorViewModel>.CreateAsync(actors, page, 30);

            var paginatedView = new PopularActorsPaginatedViewModel
            {
                Actors = paginated,
                TotalCount = actors.Count(),
            };

            return this.View(paginatedView);
        }
    }
}
