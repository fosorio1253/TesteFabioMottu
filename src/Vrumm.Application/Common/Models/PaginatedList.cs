using Microsoft.EntityFrameworkCore;

namespace Vrumm.Application.Common.Models;
public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyList<T> items, int count, int pageIndex, int pageSize)
    {
        if (pageIndex < 1) throw new ArgumentException("Page index must be greater than zero.", nameof(pageIndex));
        if (pageSize < 1) throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageIndex - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items.AsReadOnly(), count, pageIndex, pageSize);
    }
}