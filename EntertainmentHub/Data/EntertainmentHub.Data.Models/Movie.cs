namespace EntertainmentHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class Movie : BaseDeletableModel<int>
    {
        public Movie()
        {
            this.MovieReviews = new HashSet<MovieReview>();
            this.MovieCountries = new HashSet<MovieCountry>();
            this.MovieActors = new HashSet<MovieActor>();
            this.MovieComments = new HashSet<MovieComment>();
            this.Ratings = new HashSet<Rating>();
            this.Slideshow = new HashSet<MovieSlide>();
            this.MovieGenres = new HashSet<MovieGenre>();
        }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Title { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Director { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 20)]
        public string Poster { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 20)]
        public string Trailer { get; set; }

        [StringLength(150, MinimumLength = 20)]
        public string IMDBLink { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Language { get; set; }

        public int Runtime { get; set; }

        public double Budget { get; set; }

        public double Revenue { get; set; }

        public double Popularity { get; set; }

        public string Status { get; set; }

        [Required]
        public string Tagline { get; set; }

        public double AverageVote { get; set; }

        public int TotalVotes { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<MovieComment> MovieComments { get; set; }

        public virtual ICollection<MovieGenre> MovieGenres { get; set; }

        public virtual ICollection<MovieReview> MovieReviews { get; set; }

        public virtual ICollection<MovieCountry> MovieCountries { get; set; }

        public virtual ICollection<MovieActor> MovieActors { get; set; }

        public virtual ICollection<MovieSlide> Slideshow { get; set; }
    }
}
