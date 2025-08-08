using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biling.DataModels.LoginModels
{
    public  class LoginResponse
    {
        public bool Success { get; set; }
        public  string Message { get; set; }
        public  string Token { get; set; }
        public  User User { get; set; }
    }

    public class LoginRequest
    {
        public  string UserName { get; set; }
        public  string Password { get; set; }
        public  DateTime? LogInTime { get; set; }
    }

    public class User
    {
        public  int UserId { get; set; }
        public  string Username { get; set; }
        public  string Password { get; set; }
        public  string? FirstName { get; set; }
        public  string? LastName { get; set; }
        public  bool IsActive { get; set; }
        public  DateTime CreatedOn { get; set; }
        public  DateTime UpdatedOn { get; set; }
        public  int Ref_RoleId { get; set; }
        public  string Email { get; set; }
        public  string? ContactNumber { get; set; }

    }
}
