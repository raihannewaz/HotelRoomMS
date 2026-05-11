using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Identity.Users.Features.UpdateUserStatus;

public record UpdateUserStatusResponse(long Id, string CurrentStatus);
