using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Identity.Roles.Features.AddRoleClaims;

public record AddRoleClaimsRequest(long Id)
{
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
}
