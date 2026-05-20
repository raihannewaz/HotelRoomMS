using Common.Core.CommonModelProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Domain
{
    public class Room : CommonModelProperty
    {
        public long Id { get; private set; }
        public int RoomTypeId { get; private set; }
        public string RoomNumber { get; private set; }
    }
}
