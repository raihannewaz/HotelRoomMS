using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.DTOs
{
    public class FiscalYearDto
    {
        public long Id { get;  set; }
        public DateTime FromDate { get;  set; }
        public DateTime ToDate { get;  set; }
        public string Code { get;  set; }
        public bool IsClosed { get;  set; }
        public bool IsActive { get;  set; }
    }
}
