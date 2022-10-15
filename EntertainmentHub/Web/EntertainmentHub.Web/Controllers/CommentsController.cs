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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await this.commentsService.GetCommentByIdAsync<MovieCommentViewModel>(id);

            if (comment is null)
            {
                return this.NotFound();
            }

            await this.commentsService.DeleteCommentAsync(id);

            return this.RedirectToAction("Details", "Movies", new { id = comment.MovieId });
        }
    }
}
