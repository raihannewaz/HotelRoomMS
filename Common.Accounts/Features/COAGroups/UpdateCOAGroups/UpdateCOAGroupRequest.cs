using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Features.COAGroups.UpdateCOAGroups
{
    public record UpdateCOAGroupRequest
    {
        public long Id { get; init; }
        public long ParentId { get; init; }
        public string Name { get; init; }
    }
}
