using Common.Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.DTOs
{
    public class JournalMasterDto
    {
        public long Id { get;  set; }
        public string VoucherNumber { get;  set; }
        public DateTime EntryDate { get;  set; }
        public long FiscalYearId { get;  set; }
        public long ReferenceId { get;  set; }
        public string Note { get;  set; }
        public string JournalType { get;  set; }
        public string TenantId { get;  set; }

        public List<JournalDetailDto> JournalsDetail { get; set; } = new List<JournalDetailDto>();
    }
}
