namespace EntertainmentHub.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Web.ViewModels.Comments;

    public class CommentsService : ICommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentsRepository;
        private readonly IDeletableEntityRepository<MovieComment> movieCommentsRepository;

        public CommentsService(
            IDeletableEntityRepository<MovieComment> movieCommentsRepository,
            IDeletableEntityRepository<Comment> commentsRepository)
        {
            this.movieCommentsRepository = movieCommentsRepository;
            this.commentsRepository = commentsRepository;
        }

        public async Task CreateCommentAsync(MovieCommentInputModel inputModel)
        {
            var comment = new Comment
            {
                Content = inputModel.Content,
            };

            await this.commentsRepository.AddAsync(comment);
            await this.commentsRepository.SaveChangesAsync();

            var movieComment = new MovieComment
            {
                MovieId = inputModel.MovieId,
                UserId = inputModel.UserId,
                CommentId = comment.Id,
            };

            await this.movieCommentsRepository.AddAsync(movieComment);
            await this.movieCommentsRepository.SaveChangesAsync();
        }

        public IQueryable<T> GetCommentsByIdAsQueryable<T>(int id)
        {
            return this.movieCommentsRepository
                .AllAsNoTracking()
                .Where(x => x.MovieId == id)
                .To<T>();
        }
    }
}
