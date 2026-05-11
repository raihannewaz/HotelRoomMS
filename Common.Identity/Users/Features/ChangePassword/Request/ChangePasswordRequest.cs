namespace Common.Identity.Users.Features.ChangePassword.Request;

public record ChangePasswordRequest(long Id, string OldPassword, string NewPassword);
