using Common.Identity.Shared.Models;

namespace Common.Identity.Identity.Users.Dtos;

public record UserDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserState UserState { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; }
    public string UserStatus { get; set; }
}
