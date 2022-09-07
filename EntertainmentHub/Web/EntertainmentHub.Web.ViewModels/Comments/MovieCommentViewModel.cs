namespace EntertainmentHub.Web.ViewModels.Comments
{
    using System;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieCommentViewModel : IMapFrom<MovieComment>
    {
        public int MovieId { get; set; }

        public string MovieName { get; set; }

        public int UserId { get; set; }

        public string UserUsername { get; set; }

        public int? CommentId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
