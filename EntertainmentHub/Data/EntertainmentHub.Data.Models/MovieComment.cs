namespace EntertainmentHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class MovieComment : IDeletableEntity
    {
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int? CommentId { get; set; }

        public virtual Comment Comment { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
