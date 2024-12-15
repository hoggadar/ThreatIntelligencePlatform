using Microsoft.EntityFrameworkCore;

namespace ThreatIntelligencePlatformDataAccess.Pagination;

public class PaginatedList<TEntity> where TEntity : class
{
    public IEnumerable<TEntity> Items { get; set; }
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedList(IEnumerable<TEntity> items, int count, int pageIndex, int pageSize)
    {
        Items = items;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
    
    private bool HasPreviousPage => PageIndex > 1;
    private bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<TEntity>> CreateAsync(IQueryable<TEntity> source, int pageIndex, int pageSize)
    {
        var count = (int)Math.Ceiling((double)source.Count() / pageSize);
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<TEntity>(items, count, pageIndex, pageSize);
    }
}