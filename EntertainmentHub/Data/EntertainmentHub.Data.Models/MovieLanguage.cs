namespace EntertainmentHub.Data.Models
{
    using System;

    using EntertainmentHub.Data.Common.Models;

    public class MovieLanguage : IDeletableEntity
    {
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}