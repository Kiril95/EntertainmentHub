namespace EntertainmentHub.Web.ViewModels.Comments
{
    using System;

    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Services.Mapping;

    public class MovieCommentViewModel : IMapFrom<MovieComment>
    {
        public int MovieId { get; set; }

        public string MovieTitle { get; set; }

        public string UserId { get; set; }

        public string UserUsername { get; set; }

        public int? CommentId { get; set; }

        public string CommentContent { get; set; }

        public DateTime CommentCreatedOn { get; set; }
    }
}
