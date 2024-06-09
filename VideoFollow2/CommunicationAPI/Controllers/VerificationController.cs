using AutoMapper;
using Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;
using System.IdentityModel.Tokens.Jwt;

namespace CommunicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VerificationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;


        public VerificationController(IMapper mapper, IEmailService emailService)
        {
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("verificationList")]
        public async Task<IActionResult> GetAllDrivers()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
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

                    var availableRides = await statefulProxy.GetAllDrivers();

                    if (availableRides == null || !availableRides.Any())
                    {
                        return NotFound("No available rides found.");
                    }

                    return Ok(availableRides);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("verifyUser/{userId}")]
        public async Task<IActionResult> VerifyUser(string userId)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
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

                    var result = await statefulProxy.VerifyUser(userId);

                    if (!result)
                    {
                        return NotFound("User not found or already accepted.");
                    }
                    await _emailService.SendEmailAsync(userId, "Verification Successful", "Your account has been verified successfully.");
                    return Ok("User verified successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("denyUser/{userId}")]
        public async Task<IActionResult> DenyUser(string userId)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
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

                    var result = await statefulProxy.DenyUser(userId);

                    if (!result)
                    {
                        return NotFound("User not found or already denied.");
                    }
                    await _emailService.SendEmailAsync(userId, "Verification Denied", "Your account has been denied gl.");
                    return Ok("User denied successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }
    }

}
