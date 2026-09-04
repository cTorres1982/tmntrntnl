namespace JobTracker.SharedKernel.Pagination;

/// <summary>
/// Offset/page-number pagination for read-optimized queries (e.g. SearchJobsQuery).
/// For very large datasets prefer cursor-based pagination instead — see
/// DataBase/queries.sql for the cursor-based alternative and the rationale.
/// </summary>
public sealed class PagedList<T>
{
    private PagedList(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IReadOnlyList<T> Items { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;

    public static PagedList<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalCount) =>
        new(items, page, pageSize, totalCount);
}
