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
    public class BlockingController : ControllerBase
    {
        private readonly IMapper _mapper;

        public BlockingController(IMapper mapper)
        {
            _mapper = mapper;
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("blockingList")]
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
        [Route("blockUser/{userId}")]
        public async Task<IActionResult> BlockUser(string userId)
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

                    var result = await statefulProxy.BlockUser(userId);

                    if (!result)
                    {
                        return NotFound("User not found or already blocked.");
                    }

                    return Ok("User blocked successfully.");
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
        [Route("unblockUser/{userId}")]
        public async Task<IActionResult> UnblockUser(string userId)
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

                    var result = await statefulProxy.UnblockUser(userId);

                    if (!result)
                    {
                        return NotFound("User not found or already unblocked.");
                    }

                    return Ok("User unblocked successfully.");
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
