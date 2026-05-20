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





    public static IEnumerable<string> GetAll() =>
        typeof(Permissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null)!);
}
