namespace Common.Identity.Roles.Features.CreateRole;

public record CreateRoleRequest
{
    public string Name { get; set; } = null!;
}
