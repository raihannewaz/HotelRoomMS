using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class BookingGuest
    {
        public long Id { get; private set; }
        public long BookingId { get; private set; }
        public string? GuestName { get; private set; }
        public string? Relation { get; private set; }
        public string? Phone { get; private set; }
        public int Age { get; private set; }
        public bool IsPrimary { get; private set; }

        public static BookingGuest Create(long id, long bookingId, string guestName, string relation, string phone, int age, bool isPrimary)
        {
            return new BookingGuest
            {
                Id = id,
                BookingId = bookingId,
                GuestName = guestName,
                Relation = relation,
                Phone = phone,
                Age = age,
                IsPrimary = isPrimary
            };
        }

        public void Update(string guestName, string relation, string phone, int age, bool isPrimary)
        {
            GuestName = guestName;
            Relation = relation;
            Phone = phone;
            Age = age;
            IsPrimary = isPrimary;
        }

    }
}
