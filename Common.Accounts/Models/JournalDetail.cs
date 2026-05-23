using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Models
{
    public class JournalDetail
    {
        public long Id { get; private set; }
        public long JournalMasterId { get; private set; }
        public long AccountId { get; private set; }
        public decimal DebitAmount { get; private set; }
        public decimal CreditAmount { get; private set; }
        public string Note { get; private set; }
        public long ViceVersaAccountId { get; private set; }

        public static JournalDetail Create(long journalMasterId, long accountId, decimal debitAmount, decimal creditAmount, string note, long viceVersaAccountId)
        {
            return new JournalDetail
            {
                JournalMasterId = journalMasterId,
                AccountId = accountId,
                DebitAmount = debitAmount,
                CreditAmount = creditAmount,
                Note = note,
                ViceVersaAccountId = viceVersaAccountId
            };
        }

        public void Update(long accountId, decimal debitAmount, decimal creditAmount, string note, long viceVersaAccountId)
        {
            AccountId = accountId;
            DebitAmount = debitAmount;
            CreditAmount = creditAmount;
            Note = note;
            ViceVersaAccountId = viceVersaAccountId;
        }
    }
}