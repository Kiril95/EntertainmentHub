namespace EntertainmentHub.Web.ViewModels.Reviews
{
    using System;
    using System.Collections.Generic;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieReviewsCollectionViewModel : IMapFrom<Movie>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Poster { get; set; }

        public DateTime ReleaseDate { get; set; }

        public double Rating { get; set; }

        public ICollection<MovieReviewViewModel> MovieReviews { get; set; }
    }
}
