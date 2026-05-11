namespace Common.Identity.Roles.Dtos;

public record RolePermissionDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public IEnumerable<string> Permissions { get; set; }
}
