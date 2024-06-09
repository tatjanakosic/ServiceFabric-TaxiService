using AutoMapper;
using Communication;
using Communication.DTOs;
using Communication.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.WindowsAzure.Storage.File.Protocol;
using System.Fabric;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CommunicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunicationController : ControllerBase 
    {
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;


        public CommunicationController(IMapper mapper, JwtSettings jwtSettings)
        {
            _mapper = mapper;
            _jwtSettings = jwtSettings;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User details are required.");
            }

            var fabricClient = new FabricClient();
            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(
                new Uri("fabric:/VideoFollow2/ProductCatalogue"));

            userDto.Password = HashPassword(userDto.Password);

            User user = _mapper.Map<User>(userDto);
            string userMessage = null;
            string userVerificationStatus = null;
            string userBlockingStatus = null;

            foreach (var partition in partitionList)
            {
                try
                {
                    var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                    var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                        new Uri("fabric:/VideoFollow2/ProductCatalogue"), partitionKey);

                    userMessage = await statefulProxy.FetchUserType(user.Email, user.Password);
                    userVerificationStatus = await statefulProxy.FetchUserVerificationStatus(user.Email);
                    userBlockingStatus = await statefulProxy.FetchUserBlockingStatus(user.Email);

                    if (!string.IsNullOrEmpty(userMessage))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            if (string.IsNullOrEmpty(userMessage))
            {
                return Unauthorized("Invalid username or password.");
            }

            var tokenString = GenerateJwtToken(userDto.Email, userMessage, userVerificationStatus, userBlockingStatus);
            return Ok(new { Token = tokenString });
        }

        private string GenerateJwtToken(string email, string role, string userVerificationStatus, string userBlockingStatus)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                    Subject = new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.NameIdentifier, email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("VerificationStatus", userVerificationStatus),
                    new Claim("BlockingStatus", userBlockingStatus)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }





        [HttpPut]
        [AllowAnonymous]
        [Route("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileDTO profile)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                return Unauthorized("Token is invalid.");
            }

            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                return Unauthorized("Token is invalid.");
            }

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            if (emailClaim == null)
            {
                return Unauthorized("Token does not contain user information.");
            }

            var email = emailClaim.Value;

            var fabricClient = new FabricClient();
            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(
                new Uri("fabric:/VideoFollow2/ProductCatalogue"));

            foreach (var partition in partitionList)
            {
                try
                {
                    var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                    var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                        new Uri("fabric:/VideoFollow2/ProductCatalogue"), partitionKey);

                    var userProfileDTO = await statefulProxy.GetProfileDTObyEmail(email);
                    if (userProfileDTO == null)
                    {
                        return NotFound("User not found.");
                    }

                    // Update the user profile
                    userProfileDTO.Username = profile.Username;
                    userProfileDTO.Name = profile.Name;
                    userProfileDTO.LastName = profile.LastName;
                    userProfileDTO.DateOfBirth = profile.DateOfBirth;
                    userProfileDTO.Address = profile.Address;

                    bool check = await statefulProxy.UpdateProfile(userProfileDTO);
                    
                    if (check == false) {
                        return NotFound("Profile cannot be updated");

                    }


                    return Ok("Profile updated successfully");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                return Unauthorized("Token is invalid.");
            }

            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                return Unauthorized("Token is invalid.");
            }

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            if (emailClaim == null)
            {
                return Unauthorized("Token does not contain user information.");
            }

            var email = emailClaim.Value;

            var fabricClient = new FabricClient();
            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(
                    new Uri("fabric:/VideoFollow2/ProductCatalogue"));

            foreach (var partition in partitionList)
            {
                try
                {
                    var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                    var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                        new Uri("fabric:/VideoFollow2/ProductCatalogue"), partitionKey);

                    // Fetch the user based on the email from the token
                    var userProfileDTO = await statefulProxy.GetProfileDTObyEmail(email);
                    if (userProfileDTO == null)
                    {
                        return NotFound("User not found.");
                    }
                    //ProfileDTO user = _mapper.Map<ProfileDTO>(userProfileDTO);

                    return Ok(userProfileDTO);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User details are required.");
            }

            // Encrypt the password
            user.Password = HashPassword(user.Password);

            var fabricClient = new FabricClient();
            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(
                new Uri("fabric:/VideoFollow2/ProductCatalogue"));

            foreach (var partition in partitionList)
            {
                try
                {
                    var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                    var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                        new Uri("fabric:/VideoFollow2/ProductCatalogue"), partitionKey);

                    var result = await statefulProxy.Register(user.Email, user.Password, user.Username, user.Name,
                        user.LastName, user.DateOfBirth, user.Address, Convert.ToInt32(user.UserType), user.Image);

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            return Ok();
        }


        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }


}
