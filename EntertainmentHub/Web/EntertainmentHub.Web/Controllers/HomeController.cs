namespace EntertainmentHub.Web.Controllers
{
    using System.Diagnostics;

    using EntertainmentHub.Web.ViewModels;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
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
