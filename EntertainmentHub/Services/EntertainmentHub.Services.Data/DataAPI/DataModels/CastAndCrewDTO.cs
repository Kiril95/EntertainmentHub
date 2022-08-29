namespace EntertainmentHub.Services.Data.DataAPI.DataModels
{
    using System.Collections.Generic;

    public class CastAndCrewDTO
    {
        public ICollection<CastDTO> Cast { get; set; }

        public ICollection<CrewDTO> Crew { get; set; }
    }
}
