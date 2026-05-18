using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Models
{
    public class FiscalYear : CommonModelProperty
    {
        public long Id { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public string Code { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsActive { get; private set; }


        public static FiscalYear Create(long id, DateTime fromDate, DateTime toDate, string code, long userId)
        {
            return new FiscalYear
            {
                Id = id,
                FromDate = fromDate,
                ToDate = toDate,
                Code = code,
                IsClosed = false,
                IsActive = true,
                Created = DateTimeConversion.UTCToBST(),
                CreatedBy = userId
            };
        }

        public void Update(DateTime fromDate, DateTime toDate, string code, long userId)
        {
            FromDate = fromDate;
            ToDate = toDate;
            Code = code;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }

        public void CloseFiscalYear(long userId)
        {
            IsClosed = true;
            IsActive = false;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }

        public void ActiveStatus(long userId)
        {
            IsActive = !IsActive;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }
    }
}
