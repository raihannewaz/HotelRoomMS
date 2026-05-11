namespace BlogAppManage.Application.Identities.Users.Features.UpdateUsers.Requests
{
    public record UpdateUserRequest
    {
        public int Id { get; set; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string PhoneNumber { get; init; }
    }
}
