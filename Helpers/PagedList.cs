using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TweetbookApi.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public PagedList(int pageNumber, int pageSize, int totalPages, int totalCount, List<T> items)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalCount = totalCount;
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            pageSize = pageSize == -1 ? count : pageSize;
            var totalPages = 0;

            try
            {
                totalPages = (int)Math.Ceiling(count / (decimal)pageSize);
            }
            catch (System.Exception)
            {}

            var contToPreviousPage = (pageNumber - 1) * pageSize;

            var items = await source.Skip(contToPreviousPage)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<T>(pageNumber, pageSize, totalPages, count, items);
        }
    }
}