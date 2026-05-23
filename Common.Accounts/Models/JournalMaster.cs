using Common.Accounts.DTOs;
using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using Common.Core.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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


        public void AddOrUpdateDetails(IList<JournalDetail> details)
        {
            if (details is null)
                throw new ApiException("details can not be empty.", HttpStatusCode.NoContent);

            foreach (var value in details)
            {
                var existingDetails = _journalDetails.SingleOrDefault(x => x.Id > 0 && x.Id == value.Id);
                if (existingDetails == null)
                {
                    var newDetails = JournalDetail.Create(Id, value.AccountId, value.DebitAmount, value.CreditAmount, value.Note, value.ViceVersaAccountId);
                    _journalDetails.Add(newDetails);
                }
                else
                {
                    existingDetails.Update(value.AccountId, value.DebitAmount, value.CreditAmount, value.Note, value.ViceVersaAccountId);
                }
            }

            foreach (var existingDetail in _journalDetails.Where(s => s.Id > 0).ToList())
            {
                var detail = details.SingleOrDefault(x => x.Id > 0 && x.Id == existingDetail.Id);
                if (detail == null)
                {
                    _journalDetails.Remove(existingDetail);
                }
            }
        }
    }
}
