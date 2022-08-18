namespace EntertainmentHub.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EntertainmentHub.Data.Common.Models;

    public class Comment : BaseDeletableModel<int>
    {
        public Comment()
        {
            this.Comments = new HashSet<MovieComment>();
        }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Content { get; set; }

        public virtual ICollection<MovieComment> Comments { get; set; }
    }
}
