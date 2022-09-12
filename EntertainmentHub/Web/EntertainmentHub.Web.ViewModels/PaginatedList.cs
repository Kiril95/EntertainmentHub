namespace EntertainmentHub.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(List<T> items, int itemsCount, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.TotalPages = (int)Math.Ceiling(itemsCount / (double)pageSize);

            this.AddRange(items);
        }

        public int PageNumber { get; private set; }

        public int TotalPages { get; private set; }

        public bool HasPreviousPage => this.PageNumber > 1;

        public bool HasNextPage => this.PageNumber < this.TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
