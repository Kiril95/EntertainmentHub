namespace EntertainmentHub.Web.ViewModels.Actors
{
    using System;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieActorsViewModel : IMapFrom<MovieActor>
    {
        public int MovieId { get; set; }

        public string MovieTitle { get; set; }

        public string MoviePoster { get; set; }

        public DateTime MovieReleaseDate { get; set; }

        public double MovieRating { get; set; }
    }
}
