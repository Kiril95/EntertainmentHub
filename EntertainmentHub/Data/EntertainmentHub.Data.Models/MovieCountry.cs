﻿namespace EntertainmentHub.Data.Models
{
    using System;

    using EntertainmentHub.Data.Common.Models;

    public class MovieCountry : IDeletableEntity
    {
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
