namespace EntertainmentHub.Web.ViewModels.Administration.Movies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Models;

    public class CreateMovieInputModel
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
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
        [Display(Name = "IMDB Link")]
        public string IMDBLink { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Language { get; set; }

        public int Length { get; set; }

        public decimal Budget { get; set; }

        public virtual ICollection<MovieGenre> MovieGenres { get; set; }

        public virtual ICollection<MovieCountry> MovieCountries { get; set; }

        public virtual ICollection<MovieActor> MovieActors { get; set; }

        public virtual ICollection<MovieSlide> Slideshow { get; set; }
    }
}
