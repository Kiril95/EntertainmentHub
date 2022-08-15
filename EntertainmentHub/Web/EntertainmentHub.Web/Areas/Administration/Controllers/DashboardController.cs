namespace EntertainmentHub.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        [HttpGet("/Administration")]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
