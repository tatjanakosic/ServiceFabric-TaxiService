using AutoMapper;
using Communication.DTOs;
using Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Fabric;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.WindowsAzure.Storage.Table;

namespace CommunicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RideController : ControllerBase
    {
        private readonly IMapper _mapper;

        public RideController(IMapper mapper)
        {
            _mapper = mapper;
        }


        [HttpGet]
        [Authorize(Roles = "Driver")]
        [Authorize(Policy = "VerifiedCorrect")]
        [Authorize(Policy = "BlockedCorrect")]
        [Route("acceptRide/{rideId}")]
        public async Task<IActionResult> AcceptRide(int rideId)
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

                    var result = await statefulProxy.AcceptRide(email, rideId);

                    if (!result)
                    {
                        return NotFound("Ride not found or already accepted.");
                    }

                    return Ok("Ride accepted successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("adminRideHistory")]
        public async Task<IActionResult> GetAdminRideHistory()
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

                    var rideHistory = await statefulProxy.GetAdminRideHistory();

                    if (rideHistory == null || !rideHistory.Any())
                    {
                        return NotFound("No ride history found.");
                    }

                    return Ok(rideHistory);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }

        [HttpPut]
        [Authorize(Roles = "Driver")]
        [Route("driverRideHistory")]
        public async Task<IActionResult> GetDriverRideHistory()
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

                    var rideHistory = await statefulProxy.GetDriverRideHistory(email);

                    if (rideHistory == null || !rideHistory.Any())
                    {
                        return NotFound("No ride history found.");
                    }

                    return Ok(rideHistory);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }




        [HttpPut]
        [Authorize(Roles = "User")]
        [Route("rideHistory")]
        public async Task<IActionResult> GetRideHistory()
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

                    var rideHistory = await statefulProxy.GetRideHistory(email);

                    if (rideHistory == null || !rideHistory.Any())
                    {
                        return NotFound("No ride history found.");
                    }

                    return Ok(rideHistory);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return StatusCode(500, "Service partitions are unavailable.");
        }

        [HttpPut]
        [Authorize(Roles = "Driver")]
        [Authorize(Policy = "VerifiedCorrect")]
        [Authorize(Policy = "BlockedCorrect")]
        [Route("availableRides")]
        public async Task<IActionResult> GetAvailableRides()
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

                    var availableRides = await statefulProxy.GetAvailableRides();

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
        [Authorize(Roles = "User")]
        [Route("orderRide")]
        public async Task<IActionResult> OrderRide([FromBody] RideRequestDTO rideRequest)
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

            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/VideoFollow2/CustomerAnalytics"));
            var email = emailClaim.Value;

            // Process the ride request (save to database, etc.)
            var rideOrderResult = await statelessProxy.OrderRide(email, rideRequest);

            if (rideOrderResult == null)
            {
                return BadRequest("Failed to order ride.");
            }

            return Ok(new
            {
                message = "Ride ordered successfully.",
                duration = rideOrderResult.Duration,
                price = rideOrderResult.Price
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("confirmRide")]
        public async Task<IActionResult> ConfirmRide([FromBody] RideResponseDTO rideConfirmation)
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

                    var confirmResult = await statefulProxy.ConfirmRide(email, rideConfirmation);

                    if (confirmResult != null)
                    {
                        return Ok(new { message = "Ride confirmed successfully." });
                    }
                    return StatusCode(500, $"Nema bato:");

                }
                catch (Exception ex)
                {
                    return BadRequest("Failed to confirm ride.");
                }
            }
            return BadRequest("Failed to confirm ride.");

        }

        [HttpGet]
        [Route("countdown")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetCountdown()
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading token: {ex.Message}"); // Debug statement
                return Unauthorized("Token is invalid.");
            }

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            if (emailClaim == null)
            {
                return Unauthorized("Token does not contain user information.");
            }

            var tokenEmail = emailClaim.Value;
            Console.WriteLine($"Token email: {tokenEmail}"); // Debug statement

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

                    var countdown = await statefulProxy.GetCountdown(tokenEmail);
                    if (countdown != null)
                    {
                        return Ok(countdown);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in partition {partition.PartitionInformation.Id}: {ex.Message}"); // Debug statement
                }
            }

            return NotFound("No active ride found.");
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("submit")]
        public async Task<IActionResult> SubmitRating([FromBody] RatingDTO ratingDto)
        {
            if (ratingDto == null || ratingDto.Rating < 1 || ratingDto.Rating > 5)
            {
                return BadRequest("Invalid rating.");
            }

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
            var rating = ratingDto.Rating;
            var isAccepted = ratingDto.IsAccepted;


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

                    var confirmResult = await statefulProxy.RateDriver(isAccepted, rating);

                    if (confirmResult != false)
                    {
                        return Ok(new { message = "Ride rated successfully." });
                    }
                    return StatusCode(500, $"Nema bato:");

                }
                catch (Exception ex)
                {
                    return BadRequest("Failed to rate ride.");
                }
            }
            return BadRequest("Failed to rate ride.");

        }



    }
}
