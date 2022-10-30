namespace EntertainmentHub.Web.ViewModels.Actors
{
    public class PopularActorsPaginatedViewModel
    {
        public PaginatedList<PopularActorViewModel> Actors { get; set; }

        public int TotalCount { get; set; }

        public int PageResults()
        {
            int pageResults = 0;

            if (this.TotalCount < this.Actors.PageNumber * 30)
            {
                return pageResults = this.TotalCount;
            }
            else
            {
                return pageResults = this.Actors.PageNumber * 30;
            }
        }
    }
}
