using Biling.DataModels.LoginModels;
using Billing.BussinessLogic;
using Billing.BussinessLogic.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.BussinessLogic
{
    public class Login : ILoginService
    {
        private readonly IJWTService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public Login(IJWTService jwtService,IOptions<JwtSettings> jwtSettings)
        {
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Username and password are required.");
            }

            // Authenticate user (in production, use proper password hashing)
            //var user = await _userRepository.GetUserByUsernameAsync(request.Username);

            //infuture get this user from db by username and password
            var user = new User()
            {
                UserId = 1,
                Username = "prashant",
                Password = "1234",
                Role = "Admin",
                Email = "abc123@gmail.com"
            };
            if (user == null || user.Password != request.Password)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store refresh token
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            
            ////
            //Store the Refresh Token in DB For RefreshToken Method.
            //await _refreshTokenStore.SaveRefreshTokenAsync(refreshToken, user.Id, refreshTokenExpiry);
            ///
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes), 
                User = new User
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshRequest request)
        {
            //if (string.IsNullOrWhiteSpace(request.RefreshToken))
            //{
            //    throw new ArgumentException("Refresh token is required.");
            //}

            //// Validate refresh token From DB.
            //var storedRefreshToken = await _refreshTokenStore.GetRefreshTokenAsync(request.RefreshToken);
            //if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
            //{
            //    throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            //}

            //// Get user
            //var user = await _userRepository.GetUserByIdAsync(storedRefreshToken.UserId);
            //if (user == null)
            //{
            //    throw new UnauthorizedAccessException("User not found.");
            //}

            //// Generate new tokens (token rotation)
            //var newAccessToken = _jwtService.GenerateAccessToken(user);
            //var newRefreshToken = _jwtService.GenerateRefreshToken();

            //// Revoke old refresh token
            //await _refreshTokenStore.RevokeRefreshTokenAsync(request.RefreshToken);

            //// Store new refresh token
            //var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            //await _refreshTokenStore.SaveRefreshTokenAsync(newRefreshToken, user.Id, newRefreshTokenExpiry);

            //return new LoginResponse
            //{
            //    AccessToken = newAccessToken,
            //    RefreshToken = newRefreshToken,
            //    ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            //    User = new UserModel
            //    {
            //        Id = user.Id,
            //        Name = user.Name,
            //        Role = user.Role
            //    }
            //};

            throw new NotImplementedException();
        }

        public async Task LogoutAsync(RefreshRequest request)
        {
            //if (string.IsNullOrWhiteSpace(request.RefreshToken))
            //{
            //    throw new ArgumentException("Refresh token is required.");
            //}

            //// Revoke refresh token
            //await _refreshTokenStore.RevokeRefreshTokenAsync(request.RefreshToken);

            throw new NotImplementedException();

        }
    }
}
