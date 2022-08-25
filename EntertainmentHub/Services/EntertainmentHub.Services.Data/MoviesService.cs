namespace EntertainmentHub.Services.Data
{
    using System;
    using System.Threading.Tasks;
    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Web.ViewModels.Administration.Movies;
    using Microsoft.EntityFrameworkCore;

    public class MoviesService : IMoviesService
    {
        private readonly IDeletableEntityRepository<Movie> moviesRepository;

        public MoviesService(
             IDeletableEntityRepository<Movie> moviesRepository)
        {
            this.moviesRepository = moviesRepository;
        }


        public async Task CreateAsync(CreateMovieInputModel inputModel)
        {
            bool checkMovie = await this.moviesRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.Title == inputModel.Title);

            if (checkMovie)
            {
                throw new ArgumentException(string.Format($"Movie with name {inputModel.Title} already exists!"));
            }

            var movie = new Movie
            {
                Title = inputModel.Title,
                ReleaseDate = inputModel.ReleaseDate,
                Description = inputModel.Description,
                Director = inputModel.Director,
                Language = inputModel.Language,
                Length = inputModel.Length,
                Budget = inputModel.Budget,
                Poster = inputModel.Poster,
                Trailer = inputModel.Trailer,
                IMDBLink = inputModel.IMDBLink,
            };

            await this.moviesRepository.AddAsync(movie);
            await this.moviesRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var movie = await this.moviesRepository.All().FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                throw new NullReferenceException(string.Format($"There isn't a movie with this Id - {id}"));
            }

            movie.IsDeleted = true;
            movie.DeletedOn = DateTime.UtcNow;

            this.moviesRepository.Update(movie);
            await this.moviesRepository.SaveChangesAsync();
        }
    }
}
