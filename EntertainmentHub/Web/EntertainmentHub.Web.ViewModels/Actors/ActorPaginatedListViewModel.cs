namespace EntertainmentHub.Web.ViewModels.Actors
{
    public class ActorPaginatedListViewModel
    {
        public PaginatedList<ActorListViewModel> Actors { get; set; }

        public int TotalCount { get; set; }
    }
}
