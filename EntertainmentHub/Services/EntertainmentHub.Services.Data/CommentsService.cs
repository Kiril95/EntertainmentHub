namespace EntertainmentHub.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Web.ViewModels.Comments;
    using Microsoft.EntityFrameworkCore;

    public class CommentsService : ICommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentsRepository;
        private readonly IRepository<MovieComment> movieCommentsRepository;

        public CommentsService(
            IRepository<MovieComment> movieCommentsRepository,
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

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await this.movieCommentsRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.CommentId == id);

            if (comment == null)
            {
                throw new NullReferenceException(string.Format($"There isn't a comment with this Id - {id}"));
            }

            this.movieCommentsRepository.Delete(comment);
            await this.movieCommentsRepository.SaveChangesAsync();
        }

        public async Task<T> GetCommentByIdAsync<T>(int id)
        {
            return await this.movieCommentsRepository
                .AllAsNoTracking()
                .Where(x => x.CommentId == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public IQueryable<T> GetCommentsByIdAsQueryable<T>(int id)
        {
            return this.movieCommentsRepository
                .AllAsNoTracking()
                .Where(x => x.MovieId == id)
                .OrderBy(x => x.CommentId)
                .To<T>();
        }
    }
}
