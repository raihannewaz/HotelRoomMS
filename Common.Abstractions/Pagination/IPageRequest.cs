using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Abstractions.Pagination
{
    public interface IPageRequest
    {
        IList<string>? Includes { get; init; }
        IList<FilterModel>? Filters { get; init; }
        IList<string>? Sorts { get; init; }
        int Page { get; init; }
        int PageSize { get; init; }
    }
}
