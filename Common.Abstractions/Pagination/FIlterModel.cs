using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Abstractions.Pagination
{
    public record FilterModel(string FieldName, string Comparision, string FieldValue);
}
