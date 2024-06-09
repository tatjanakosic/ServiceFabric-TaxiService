using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Communication.DTOs;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CustomerAnalytics
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class CustomerAnalytics : StatelessService, IStatelessInterface
    {
        AzureStorageHelper storageHelper = new AzureStorageHelper("UseDevelopmentStorage=true", "customers");
        public CustomerAnalytics(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<RideResponseDTO> OrderRide(string email, RideRequestDTO rideRequest)
        {
            string startAdress = rideRequest.StartAdress;
            string endAdress = rideRequest.EndAdress;
            Random rand = new Random();
            int duration = rand.Next(1, 100);
            int price = rand.Next(100, 10000);

            RideResponseDTO rideResponseDTO = new RideResponseDTO(startAdress, endAdress, duration, price);
            if(rideResponseDTO != null )
                return rideResponseDTO;
            return null;

        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
