namespace Common.Identity.Roles.Features.UpdateRole;

public record UpdateRoleRequest
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}
