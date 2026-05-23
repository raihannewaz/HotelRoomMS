using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Payments.Dto
{
    public class PaymentDto
    {
        public long Id { get;  set; }
        public long ReferenceId { get;  set; }
        public DateTime PaymentDate { get;  set; }
        public decimal Amount { get;  set; }
        public string PaymentMethod { get;  set; }
        public string Remarks { get;  set; }
    }
}
