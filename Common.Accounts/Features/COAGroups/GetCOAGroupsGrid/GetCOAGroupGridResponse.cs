using Common.Accounts.DTOs;
using Common.Core.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Features.COAGroups.GetCOAGroupsGrid;

public record GetCOAGroupGridResponse(ListResultModel<COAGroupDto> COAGroups);
