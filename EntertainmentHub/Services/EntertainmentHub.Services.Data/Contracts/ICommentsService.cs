namespace EntertainmentHub.Services.Data.Contracts
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Web.ViewModels.Comments;

    public interface ICommentsService
    {
        Task CreateCommentAsync(MovieCommentInputModel inputModel);

        IQueryable<T> GetCommentsByIdAsQueryable<T>(int id);
    }
}
