using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Features.COAGroups.CreateCOAGroups
{
    public record CreateCOAGroupRequest
    {
        public long ParentId { get; init; }
        public string Name { get; init; }
    }
}
