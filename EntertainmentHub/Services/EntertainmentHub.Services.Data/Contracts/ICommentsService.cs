namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using EntertainmentHub.Web.ViewModels.Comments;

    public interface ICommentsService
    {
        Task CreateCommentAsync(MovieCommentInputModel inputModel);
    }
}
