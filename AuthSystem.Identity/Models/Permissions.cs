namespace AuthSystem.Identity.Models;

/// <summary>
/// All permission strings as constants.
/// Reference anywhere: [HasPermission(Permissions.UsersView)]
/// Add your own groups below — they are auto-discovered by DbSeeder.
/// </summary>
public static class Permissions
{
    // ── Users ───────────────────────────────────────────────
    public const string UsersView   = "Permissions.Users.View";
    public const string UsersCreate = "Permissions.Users.Create";
    public const string UsersEdit   = "Permissions.Users.Edit";
    public const string UsersDelete = "Permissions.Users.Delete";

    // ── Roles ───────────────────────────────────────────────
    public const string RolesView   = "Permissions.Roles.View";
    public const string RolesCreate = "Permissions.Roles.Create";
    public const string RolesEdit   = "Permissions.Roles.Edit";
    public const string RolesDelete = "Permissions.Roles.Delete";

    // ── Hotels ──────────────────────────────────────────────
    public const string HotelView = "Permissions.Hotel.View";
    public const string HotelCreate = "Permissions.Hotel.Create";
    public const string HotelEdit = "Permissions.Hotel.Edit";
    public const string HotelDelete = "Permissions.Hotel.Delete";

    // ── Customers ──────────────────────────────────────────────
    public const string CustomerView = "Permissions.Customer.View";
    public const string CustomerCreate = "Permissions.Customer.Create";
    public const string CustomerEdit = "Permissions.Customer.Edit";
    public const string CustomerDelete = "Permissions.Customer.Delete";

    //── Rooms ──────────────────────────────────────────────
    public const string RoomView = "Permissions.Room.View";
    public const string RoomCreate = "Permissions.Room.Create";
    public const string RoomEdit = "Permissions.Room.Edit";
    public const string RoomDelete = "Permissions.Room.Delete";

    //── RoomTypes ──────────────────────────────────────────────
    public const string RoomTypeView = "Permissions.RoomType.View";
    public const string RoomTypeCreate = "Permissions.RoomType.Create";
    public const string RoomTypeEdit = "Permissions.RoomType.Edit";
    public const string RoomTypeDelete = "Permissions.RoomType.Delete";


    //── Booking ──────────────────────────────────────────────
    public const string BookingView = "Permissions.Booking.View";
    public const string BookingCreate = "Permissions.Booking.Create";
    public const string BookingEdit = "Permissions.Booking.Edit";
    public const string BookingDelete = "Permissions.Booking.Delete";


    public static IEnumerable<string> GetAll() =>
        typeof(Permissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null)!);
}
