using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Identity.Roles.Features.GetAllRolesForUser;

public record GetAllRolesForUserRespose(List<string> Permissions);
