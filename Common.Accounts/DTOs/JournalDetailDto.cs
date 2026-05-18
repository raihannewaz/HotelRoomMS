using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.DTOs
{
    public class JournalDetailDto
    {
        public long Id { get;  set; }
        public long JournalMasterId { get;  set; }
        public long AccountId { get;  set; }
        public decimal DebitAmount { get;  set; }
        public decimal CreditAmount { get;  set; }
        public string Note { get;  set; }
        public long ViceVersaAccountId { get;  set; }
    }
}
