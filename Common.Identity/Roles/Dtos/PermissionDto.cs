namespace Common.Identity.Roles.Dtos;

public record PermissionDto
{
    public string Module { get; set; }
    public string SubModule { get; set; }
    public string Text { get; set; }
    public string Value { get; set; }
}
