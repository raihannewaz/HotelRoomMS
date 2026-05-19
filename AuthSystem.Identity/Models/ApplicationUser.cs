using Microsoft.AspNetCore.Identity;

namespace AuthSystem.Identity.Models;

/// <summary>
/// Your application user. Add any custom fields here.
/// Run migrations from your API project after changes.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
