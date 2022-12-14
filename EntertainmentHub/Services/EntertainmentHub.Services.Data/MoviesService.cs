namespace EntertainmentHub.Services.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Web.ViewModels.Administration.Movies;
    using EntertainmentHub.Web.ViewModels.Movies;
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
                Runtime = inputModel.Runtime,
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

        public IQueryable<T> GetAllMoviesAsQueryable<T>()
        {
            return this.moviesRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.ReleaseDate.Year)
                .ThenByDescending(x => x.CreatedOn)
                .To<T>();
        }

        public async Task<T> GetRandomMovieForBannerAsync<T>()
        {
            var movies = this.GetAllMoviesAsQueryable<MovieViewModel>();

            // A clever way to generate a random entity: something.OrderBy(r => Guid.NewGuid())
            Random random = new Random();
            int id = random.Next(1, movies.Count() + 1);

            return await this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public IQueryable<T> GetMoviesByGenreAsQueryable<T>(string name)
        {
            return this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.MovieGenres.Any(x => x.Genre.Name == name))
                .OrderByDescending(x => x.ReleaseDate.Year)
                .To<T>();
        }

        public IQueryable<T> GetMoviesByCountryAsQueryable<T>(string name)
        {
            return this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.MovieCountries.Any(x => x.Country.Name == name))
                .OrderByDescending(x => x.ReleaseDate.Year)
                .To<T>();
        }

        public async Task<T> GetMovieByIdAsync<T>(int id)
        {
            return await this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetRecentMoviesAsync<T>()
        {
            return await this.moviesRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .Take(13)
                .To<T>()
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPopularMoviesAsync<T>()
        {
            return await this.moviesRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.Popularity)
                .Take(13)
                .To<T>()
                .ToListAsync();
        }

        public IQueryable<T> GetTopRatedMoviesAsQueryable<T>()
        {
            return this.moviesRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.Ratings.Average(x => x.Rate))
                .Take(13)
                .To<T>();
        }

        public async Task<IEnumerable<T>> GetLatestMoviesAsync<T>()
        {
            return await this.moviesRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.ReleaseDate.Year)
                .ThenByDescending(x => x.CreatedOn)
                .Take(10)
                .To<T>()
                .ToListAsync();
        }

        public IQueryable<T> GetMoviesByYearAsQueryable<T>(int year)
        {
            return this.moviesRepository
                .AllAsNoTracking()
                .Where(x => x.ReleaseDate.Year == year)
                .To<T>();
        }
    }
}
