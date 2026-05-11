namespace Common.Core.Query;

public static class PagedQueryHelper
{
    public const string Offset = "Offset";

    public const string Next = "Next";

    public static PageData GetPageData(int page, int pageSize)
    {
        int pageNumber;
        int offset;
        if (page <= 0 ||
            pageSize <= 0)
        {
            pageNumber = int.MaxValue;
            offset = 0;
        }
        else
        {
            pageNumber = page;
            offset = (page - 1) * pageSize;
        }

        int next;
        if (pageSize <= 0)
        {
            pageSize = next = int.MaxValue;
        }
        else
        {
            pageSize = next = pageSize;
        }

        return new PageData(offset, next, pageNumber, pageSize);
    }

    public static string AppendPageStatement(string sql)
    {
        return $"{sql} " +
               $"OFFSET @{Offset} ROWS FETCH NEXT @{Next} ROWS ONLY; ";
    }

    public static ListResultModel<T> CreatePagedResponse<T>(List<T> pagedData, PageData validFilter, int totalRecords)
        where T : notnull
    {
        var totalPages = (double)totalRecords / validFilter.PageSize;

        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        return new ListResultModel<T>(pagedData, roundedTotalPages, validFilter.Page, validFilter.PageSize, totalRecords);
    }
}
