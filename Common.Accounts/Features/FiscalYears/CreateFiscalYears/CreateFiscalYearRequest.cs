using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Features.FiscalYears.CreateFiscalYears
{
    public record CreateFiscalYearRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }        

    }
}
