namespace EntertainmentHub.Web.ViewModels.Movies
{
    using System;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieSimpleViewModel : IMapFrom<Movie>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Poster { get; set; }

        public DateTime ReleaseDate { get; set; }

        public double Rating { get; set; }
    }
}
