namespace Common.Identity.Roles.Dtos;

public record GetRoleDto
{
    public long Id { get; init; }
    public string Name { get; init; }
}
