using Common.Core.CommonModelProperties;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.CustomIdentity.Models
{
    public class User : CommonModelProperty
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public bool EmailConfirmed { get; private set; } = false;
        public string UserName { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public string PhoneNumber { get; private set; } = string.Empty;
        public bool PhoneNumberConfirmed { get; private set; } = false;


        public static User Create(string firstName, string lastName, string email, string phoneNumber, string userName, string passwordHash)
        {
            var user = new User();
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.PhoneNumber = phoneNumber;
            user.UserName = userName;
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, passwordHash);

            return user;
        }

        public void Update(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }

        public void PasswordChange(string passwordHash)
        {
            PasswordHash = new PasswordHasher<User>().HashPassword(this, passwordHash);
        }

    }
}
