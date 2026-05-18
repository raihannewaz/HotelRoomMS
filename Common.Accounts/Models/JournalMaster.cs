using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Models
{
    public class JournalMaster : CommonModelProperty
    {
        public long Id { get; private set; }
        public string VoucherNumber { get; private set; }
        public DateTime EntryDate { get; private set; }
        public long FiscalYearId { get; private set; }
        public long ReferenceId { get; private set; }
        public string Note { get; private set; }
        public string JournalType { get; private set; }
        public string TenantId { get; private set; }

        private readonly List<JournalDetail> _journalDetails = new();
        public IReadOnlyCollection<JournalDetail> JournalsDetail => _journalDetails.AsReadOnly();

        public static JournalMaster Create(long id, string voucherNumber, DateTime entryDate, long fiscalYearId, long referenceId, string note, string journalType, string tenantId, long userId)
        {
            return new JournalMaster
            {
                Id = id,
                VoucherNumber = voucherNumber,
                EntryDate = entryDate,
                FiscalYearId = fiscalYearId,
                ReferenceId = referenceId,
                Note = note,
                JournalType = journalType,
                TenantId = tenantId,
                CreatedBy = userId,
                Created = DateTimeConversion.UTCToBST()
            };
        }

        public void Update(string voucherNumber, DateTime entryDate, long fiscalYearId, long referenceId, string note, string journalType, string tenantId, long userId)
        {
            VoucherNumber = voucherNumber;
            EntryDate = entryDate;
            FiscalYearId = fiscalYearId;
            ReferenceId = referenceId;
            Note = note;
            JournalType = journalType;
            TenantId = tenantId;
            ModifiedBy = userId;
            LastModified = DateTimeConversion.UTCToBST();
        }
    }
}
