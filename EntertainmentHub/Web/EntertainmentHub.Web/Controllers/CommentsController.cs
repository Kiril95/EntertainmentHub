namespace EntertainmentHub.Web.Controllers
{
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CommentsController : Controller
    {
        private readonly ICommentsService commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            this.commentsService = commentsService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComment(MovieCommentInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.commentsService.CreateCommentAsync(inputModel);

            // Redirect to current movie page
            return this.RedirectToAction("Details", "Movies", new { id = inputModel.MovieId });
        }
    }
}
