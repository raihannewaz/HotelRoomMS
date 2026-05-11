namespace Common.Core.Query;

public record ListResultModel<T>(List<T> Items, long TotalPages, int Page, int PageSize, long TotalItems)
    where T : notnull
{
    public static ListResultModel<T> Empty => new(Enumerable.Empty<T>().ToList(), 0, 0, 0, 0);

    public static ListResultModel<T> Create(List<T> items, long totalPages = 0, int page = 1, int pageSize = 20, long totalItems = 0)
    {
        return new ListResultModel<T>(items, totalPages, page, pageSize, totalItems);
    }

    public ListResultModel<U> Map<U>(Func<T, U> map)
    {
        return ListResultModel<U>.Create(
            Items.Select(map).ToList(), TotalPages, Page, PageSize, TotalItems);
    }
}
