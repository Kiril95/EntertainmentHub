namespace EntertainmentHub.Web.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels;
    using EntertainmentHub.Web.ViewModels.Movies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;

    public class HomeController : BaseController
    {
        private readonly IMoviesService moviesService;
        private readonly IDistributedCache cache;

        public HomeController(
            IMoviesService moviesService,
            IDistributedCache cache)
        {
            this.moviesService = moviesService;
            this.cache = cache;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = new TimeSpan(1, 0, 0),             
            };

            // Recent
            var recentMoviesCache = await this.cache.GetStringAsync("RecentMoviesCache");
            var recentMovies = Enumerable.Empty<MovieHomeViewModel>().AsEnumerable();

            if (recentMoviesCache is null)
            {
                recentMovies = await this.moviesService.GetRecentMoviesAsync<MovieHomeViewModel>();

                await this.cache.SetStringAsync("RecentMoviesCache", JsonConvert.SerializeObject(recentMovies), cacheOptions);
            }
            else
            {
                recentMovies = JsonConvert.DeserializeObject<IEnumerable<MovieHomeViewModel>>(recentMoviesCache);
            }

            // Popular
            var popularMoviesCache = await this.cache.GetStringAsync("PopularMoviesCache");
            var popularMovies = Enumerable.Empty<MovieHomeViewModel>().AsEnumerable();

            if (popularMoviesCache is null)
            {
                popularMovies = await this.moviesService.GetPopularMoviesAsync<MovieHomeViewModel>();

                await this.cache.SetStringAsync("PopularMoviesCache", JsonConvert.SerializeObject(popularMovies), cacheOptions);
            }
            else
            {
                popularMovies = JsonConvert.DeserializeObject<IEnumerable<MovieHomeViewModel>>(popularMoviesCache);
            }

            // Latest
            var latestMoviesCache = await this.cache.GetStringAsync("LatestMoviesCache");
            var latestMovies = Enumerable.Empty<MovieSimpleViewModel>().AsEnumerable();

            if (latestMoviesCache is null)
            {
                latestMovies = await this.moviesService.GetLatestMoviesAsync<MovieSimpleViewModel>();

                await this.cache.SetStringAsync("LatestMoviesCache", JsonConvert.SerializeObject(latestMovies), cacheOptions);
            }
            else
            {
                latestMovies = JsonConvert.DeserializeObject<IEnumerable<MovieSimpleViewModel>>(latestMoviesCache);
            }

            // I don't cache the top rated movies, because I want up to date info of the User's ratings
            var topMovies = this.moviesService.GetTopRatedMoviesAsQueryable<MovieHomeViewModel>();

            var viewModel = new HomepageViewModel 
            {
                RecentMovies = recentMovies,
                PopularMovies = popularMovies,
                TopRatedMovies = topMovies,
                LatestMovies = latestMovies,
            };

            return this.View(viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult CookiePolicy()
        {
            return this.View();
        }

        public IActionResult Credits()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [Route("/Home/ErrorView/{status:int}")]
        public IActionResult ErrorView(int status)
        {
            return this.View("~/Views/Shared/Error404.cshtml");
        }
    }
}
