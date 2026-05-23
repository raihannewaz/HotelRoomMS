using Common.Core.CommonModelProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class Payment : CommonModelProperty
    {
        public long Id { get; private set; }
        public long ReferenceId { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal Amount { get; private set; }
        public string PaymentMethod { get; private set; }
        public string Remarks { get; private set; }


        public static Payment Create(long id, long referenceId, DateTime paymentDate, decimal amount, string paymentMethod, string remarks)
        {
            return new Payment
            {
                Id = id,
                ReferenceId = referenceId,
                PaymentDate = paymentDate,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Remarks = remarks

            };
        }

        public void Update(long referenceId, DateTime paymentDate, decimal amount, string paymentMethod, string remarks)
        {
            ReferenceId = referenceId;
            PaymentDate = paymentDate;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Remarks = remarks;
        }
    }
}
