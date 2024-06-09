using Communication.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Communication.JWT
{
    public class JWTService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        private readonly AzureStorageHelper _storageHelper;

        public JWTService(JwtSettings jwtSettings, IConfiguration configuration)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _storageHelper = new AzureStorageHelper("UseDevelopmentStorage=true", "customers");
        }
        public string GenerateToken(UserDTO userDto)
        {
            // Retrieve the user from storage
            User retrievedUser = _storageHelper.RetrieveEntity<User>("Users", userDto.Email);

            if (retrievedUser == null)
            {
                throw new Exception("User not found.");
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.Email, retrievedUser.Email),
        new Claim("VerificationStatus", retrievedUser.VerificationStatus.ToString()),
        new Claim(ClaimTypes.Role, retrievedUser.CheckType(retrievedUser.UserType))
    };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

    }
}
