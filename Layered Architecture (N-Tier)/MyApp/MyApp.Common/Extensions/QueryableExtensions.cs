namespace MyApp.Common.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Paging helper: applies Skip + Take to an IQueryable.
    /// </summary>
    public static IQueryable<T> Paged<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}