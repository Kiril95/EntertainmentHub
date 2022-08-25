namespace EntertainmentHub.Web.ViewModels.Administration.Collector
{
    using System.ComponentModel.DataAnnotations;

    public class GetDataInputModel
    {
        [Range(0, int.MaxValue)]
        [Display(Name = "Start ID")]
        public int StartIndex { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "End ID")]
        public int EndIndex { get; set; }
    }
}
