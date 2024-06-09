using AutoMapper;
using Communication.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface IStatefulInterface : IService
    {
        //Global
        Task<string> Login(string email, string password);

        Task<string> Register(string email, string password, string username, string name, string lastName, DateTime dateOfBirth, string address, int userType, string image);

        Task<string> FetchUserType(string email, string password);
        Task<string> FetchUserVerificationStatus(string email);
        Task<string> FetchUserBlockingStatus(string email);

        Task<ProfileDTO> GetProfileDTObyEmail(string email);


        //User
        Task<string> ConfirmRide(string email, RideResponseDTO rideConfirmation);

        Task<bool> UpdateProfile(ProfileDTO profile);

        Task<List<RideTableDTO>> GetRideHistory(string email);

        //Driver
        Task<List<RideTableDTO>> GetAvailableRides();
        Task<bool> AcceptRide(string email, int rideId);
        Task<List<RideTableDTO>> GetDriverRideHistory(string email);

        //Admin
        Task<List<RideTableDTO>> GetAdminRideHistory();
        Task<List<ProfileDTO>> GetAllDrivers();
        Task<bool> VerifyUser(string userId);
        Task<bool> DenyUser(string userId);

        Task<bool> BlockUser(string userId);
        Task<bool> UnblockUser(string userId);

        Task<bool> RateDriver(string isAccepted, int rating);
        Task<CountdownDTO> GetCountdown(string email);
    }
}
