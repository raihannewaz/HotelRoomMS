namespace Common.Core.Query;

public struct PageData
{
    public int? Offset { get; }

    public int? Next { get; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public PageData(int? offset, int? next, int page, int pageSize)
    {
        this.Offset = offset;
        this.Next = next;
        this.Page = page;
        this.PageSize = pageSize;
    }
}
