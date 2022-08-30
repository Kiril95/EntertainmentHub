namespace EntertainmentHub.Services.Data.DataAPI
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Data.Models.Enumerations;
    using EntertainmentHub.Services.Data.DataAPI.DataModels;
    using Microsoft.EntityFrameworkCore;

    public class CollectService : ICollectService
    {
        private const string FixedImageSizePath = "https://www.themoviedb.org/t/p/w600_and_h900_bestv2";
        private const string IMDBMoviePath = "https://www.imdb.com/title/";
        private const string OriginalImageSizePath = "https://www.themoviedb.org/t/p/original";

        private readonly IDataService dataService;
        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IDeletableEntityRepository<Actor> actorsRepository;
        private readonly IDeletableEntityRepository<Genre> genresRepository;
        private readonly IDeletableEntityRepository<Language> languagesRepository;
        private readonly IDeletableEntityRepository<Country> countriesRepository;
        private readonly IRepository<Review> reviewsRepository;

        public CollectService(
            IDataService dataService,
            IDeletableEntityRepository<Movie> moviesRepository,
            IDeletableEntityRepository<Actor> actorsRepository,
            IDeletableEntityRepository<Genre> genresRepository,
            IDeletableEntityRepository<Language> languagesRepository,
            IDeletableEntityRepository<Country> countriesRepository,
            IRepository<Review> reviewsRepository)
        {
            this.dataService = dataService;
            this.moviesRepository = moviesRepository;
            this.actorsRepository = actorsRepository;
            this.genresRepository = genresRepository;
            this.languagesRepository = languagesRepository;
            this.countriesRepository = countriesRepository;
            this.reviewsRepository = reviewsRepository;
        }

        public async Task<int> AddMoviesToDatabaseAsync(int startIndex, int endIndex)
        {
            int moviesAdded = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                var movieData = await this.dataService.GetMovieDataAsync(i);

                // Get movies that are supposed to be more popular
                if (movieData is not null && movieData.Title is not null && movieData.Poster is not null &&
                    movieData.IMDBPathId is not null && movieData.Overview is not null &&
                    movieData.Runtime > 60 && movieData.TotalVotes > 1500 &&
                    DateTime.ParseExact(movieData.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).Year > 1990)
                {
                    var trailers = await this.dataService.GetMovieTrailersAsync(movieData.Id);
                    var officialTrailer = trailers.Trailers.Select(x => x.Path)?.FirstOrDefault();

                    // Filter the ISO because we want only English 'Title' names on the photos or without any at all
                    var backdrops = await this.dataService.GetMoviePhotoSlidesAsync(movieData.Id);
                    var filteredBackdrops = backdrops.Backdrops
                        .Where(x => x.ISO is null)
                        .Take(4) ?? Enumerable.Empty<SlideDTO>();

                    var castAndCrew = await this.dataService.GetCastAndCrewAsync(movieData.Id);
                    var director = castAndCrew.Crew.FirstOrDefault(x => x.Job == "Director").Name;

                    var reviews = await this.dataService.GetMovieReviewAsync(movieData.Id);

                    var movie = new Movie
                    {
                        TMDBId = movieData.Id,
                        Title = movieData.Title,
                        Director = director,
                        ReleaseDate = DateTime.ParseExact(movieData.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Poster = $"{FixedImageSizePath}{movieData.Poster}",
                        IMDBLink = $"{IMDBMoviePath}{movieData.IMDBPathId}",
                        Trailer = officialTrailer,
                        Budget = movieData.Budget,
                        Revenue = movieData.Revenue,
                        Runtime = movieData.Runtime,
                        Status = movieData.Status,
                        Tagline = movieData.Tagline,
                        Description = movieData.Overview,
                        Popularity = movieData.Popularity,
                        TotalVotes = movieData.TotalVotes,
                        AverageVote = movieData.AverageVote,
                    };

                    foreach (var genre in movieData.Genres)
                    {
                        var targetGenre = await this.genresRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Name == genre.Name);

                        if (targetGenre is null)
                        {
                            targetGenre = new Genre { Name = genre.Name };

                            await this.genresRepository.AddAsync(targetGenre);
                            await this.genresRepository.SaveChangesAsync();
                        }

                        movie.MovieGenres.Add(new MovieGenre { GenreId = targetGenre.Id });
                    }

                    foreach (var language in movieData.Languages)
                    {
                        var targetLanguage = this.languagesRepository.AllAsNoTracking().FirstOrDefault(x => x.Name == language.Name);

                        if (targetLanguage == null)
                        {
                            targetLanguage = new Language { Name = language.Name };

                            await this.languagesRepository.AddAsync(targetLanguage);
                            await this.languagesRepository.SaveChangesAsync();
                        }

                        movie.Languages.Add(new MovieLanguage { LanguageId = targetLanguage.Id });
                    }

                    foreach (var country in movieData.Countries)
                    {
                        var targetCountry = await this.countriesRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Name == country.Name);

                        if (targetCountry is null)
                        {
                            targetCountry = new Country { Name = country.Name };

                            await this.countriesRepository.AddAsync(targetCountry);
                            await this.countriesRepository.SaveChangesAsync();
                        }

                        movie.MovieCountries.Add(new MovieCountry { CountryId = targetCountry.Id });
                    }

                    foreach (var slide in filteredBackdrops)
                    {
                        movie.Slideshow.Add(new MovieSlide { Path = $"{OriginalImageSizePath}{slide.FilePath}" });
                    }

                    foreach (var cast in castAndCrew.Cast.Take(10))
                    {
                        var currentActor = await this.dataService.GetActorAsync(cast.ActorId);

                        var targetActor = await this.actorsRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Name == currentActor.Name);

                        if (targetActor is null)
                        {
                            string birthplace = currentActor.Birthplace is not null &&
                                currentActor.Birthplace.Length > 50 ?
                                currentActor.Birthplace.Substring(0, 50) : currentActor.Birthplace;

                            targetActor = new Actor
                            {
                                Name = currentActor.Name,
                                Biography = currentActor.Biography,
                                Gender = (Gender)currentActor.Gender,
                                Birthplace = birthplace,
                                DateOfBirth = currentActor.Birthday is not null ? DateTime.ParseExact(currentActor.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                                DateOfDeath = currentActor.Deathday is not null ? DateTime.ParseExact(currentActor.Deathday, "yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                                Photo = $"{FixedImageSizePath}{currentActor.Photo}",
                                Popularity = currentActor.Popularity,
                            };

                            await this.actorsRepository.AddAsync(targetActor);
                            await this.actorsRepository.SaveChangesAsync();
                        }

                        movie.MovieActors.Add(new MovieActor
                        {
                            ActorId = targetActor.Id,
                            CharacterPlayed = cast.CharacterName,
                        });
                    }

                    foreach (var reviewDTO in reviews.Reviews)
                    {
                        var targetReview = await this.reviewsRepository.AllAsNoTracking()
                            .FirstOrDefaultAsync(x => x.AuthorUsername == reviewDTO.AuthorDetails.Username);

                        if (targetReview is null)
                        {
                            targetReview = new Review
                            {
                                AuthorName = reviewDTO.AuthorReviewName,
                                AuthorUsername = reviewDTO.AuthorDetails.Username,
                                AvatarPath = string.IsNullOrWhiteSpace(reviewDTO.AuthorDetails.Avatar) ? null : reviewDTO.AuthorDetails.Avatar.Substring(1),
                                Content = reviewDTO.Content,
                            };

                            await this.reviewsRepository.AddAsync(targetReview);
                            await this.reviewsRepository.SaveChangesAsync();
                        }

                        if (!movie.MovieReviews.Any(x => x.ReviewId == targetReview.Id))
                        {
                            movie.MovieReviews.Add(new MovieReview { ReviewId = targetReview.Id });
                        }
                    }

                    await this.moviesRepository.AddAsync(movie);
                    await this.moviesRepository.SaveChangesAsync();
                    moviesAdded++;
                }
            }

            return moviesAdded;
        }

        public Task AddTVShowsToDatabaseAsync(int startIndex, int endIndex)
        {
            throw new NotImplementedException();
        }
    }
}
