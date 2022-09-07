namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Web.ViewModels.Comments;

    public class MovieViewModel : IMapFrom<Movie>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public string Tagline { get; set; }

        public string Poster { get; set; }

        public string Trailer { get; set; }

        public string Director { get; set; }

        public string IMDBLink { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int Runtime { get; set; }

        public double Budget { get; set; }

        public double Revenue { get; set; }

        public double Popularity { get; set; }

        public string Status { get; set; }

        public ICollection<MovieGenresViewModel> Genres { get; set; }

        public ICollection<MovieCountriesViewModel> Countries { get; set; }

        public ICollection<MovieLanguageViewModel> Languages { get; set; }

        public ICollection<MovieCastViewModel> Cast { get; set; }

        public ICollection<MovieReviewViewModel> Reviews { get; set; }

        public ICollection<MovieCommentViewModel> Comments { get; set; }
    }
}
