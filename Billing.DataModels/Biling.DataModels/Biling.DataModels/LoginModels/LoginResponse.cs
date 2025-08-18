using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Biling.DataModels.LoginModels
{
    public  class LoginResponse
    {
        public bool Success { get; set; }
        public  string? Message { get; set; }
        public  string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiry { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry {  get; set; }
        public User? User { get; set; }
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

        [JsonIgnore]
        public  string? Password { get; set; }
        public  string? FirstName { get; set; }
        public  string? LastName { get; set; }
        public  bool IsActive { get; set; }
        public  DateTime CreatedOn { get; set; }
        public  DateTime UpdatedOn { get; set; }
        public  string? Role { get; set; }
        public  string Email { get; set; }
        public  string? ContactNumber { get; set; }

    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
