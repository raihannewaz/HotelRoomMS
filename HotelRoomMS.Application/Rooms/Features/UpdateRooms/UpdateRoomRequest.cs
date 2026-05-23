namespace HotelRoomMS.Application.Rooms.Features.UpdateRooms
{
    public record UpdateRoomRequest
    {
        public long Id { get; init; }
        public long HotelId { get; init; }
        public long RoomTypeId { get; init; }
        public string RoomNumber { get; init; }
        public decimal PricePerDay { get; init; }
    }
}
