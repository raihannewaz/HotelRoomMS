namespace Common.CustomIdentity.Dto
{
    public class UserDto
    {
        public int Id { get;  init; }
        public string FirstName { get;  init; }  
        public string LastName { get;  init; }  
        public string Email { get;  init; }  
        public bool EmailConfirmed { get;  init; }
        public string UserName { get;  init; }  
        public string Password { get;  init; }  
        public bool IsActive { get;  init; }
        public string PhoneNumber { get;  init; }  
        public bool PhoneNumberConfirmed { get;  init; }
    }
}
