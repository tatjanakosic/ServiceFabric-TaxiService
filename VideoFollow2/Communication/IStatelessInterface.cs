using Communication.DTOs;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.WindowsAzure.Storage.File.Protocol;

namespace Communication
{
    public interface IStatelessInterface : IService
    {

        Task<RideResponseDTO> OrderRide(string email, RideRequestDTO rideRequest);

    }
}
