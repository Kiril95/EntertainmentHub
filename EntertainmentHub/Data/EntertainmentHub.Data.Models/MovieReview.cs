namespace EntertainmentHub.Data.Models
{
    using System;

    using EntertainmentHub.Data.Common.Models;

    public class MovieReview : IAuditInfo
    {
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int? ReviewId { get; set; }

        public virtual Review Review { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
