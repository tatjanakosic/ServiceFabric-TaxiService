using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Communication;
using Communication.DTOs;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.WindowsAzure.Storage.Table;
using static System.Net.Mime.MediaTypeNames;

namespace ProductCatalogue
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalogue : StatefulService, IStatefulInterface
    {
        AzureStorageHelper storageHelper = new AzureStorageHelper("UseDevelopmentStorage=true", "customers");
        AzureStorageHelper storageHelper2 = new AzureStorageHelper("UseDevelopmentStorage=true", "rides");
        private readonly IMapper _mapper;


        public ProductCatalogue(StatefulServiceContext context, IMapper mapper)
            : base(context)
        { _mapper = mapper; }


        public async Task<string> Login(string email, string password)
        {


            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                return "Success";
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return "No Succ";
            }

        }

        public async Task<string> FetchUserType(string email, string password)
        {


            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                return retrievedCustomer.CheckType(retrievedCustomer.UserType);
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return "";
            }

        }


        public async Task<string> Register(string email, string password, string username, string name, string lastName, DateTime dateOfBirth, string address, int userType, string image)
        {
            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                return "ExistsD";
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                User customer = new User(email, password, username, name, lastName, dateOfBirth, address, userType, image);
                ServiceEventSource.Current.ServiceMessage(this.Context, $"{customer.Email} {customer.Password} {customer.UserType}");
                storageHelper.InsertEntity(customer);
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer entity inserted.");
                return "Customer entity inserted.D";
            }

        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task<CountdownDTO> GetCountdown(string email)
        {
            var query = new TableQuery<Ride>()
                .Where(TableQuery.GenerateFilterCondition("IsAccepted", QueryComparisons.Equal, email))
                .Where(TableQuery.GenerateFilterConditionForInt("IsFinished", QueryComparisons.Equal, 0));

            var rides = await storageHelper2.ExecuteQueryAsync(query);

            var ride = rides.FirstOrDefault();
            if (ride == null)
            {
                return null;
            }

            var countdown = new CountdownDTO
            {
                WaitDuration = ride.WaitDuration,
                RideDuration = ride.RideDuration,
                IsAccepted = ride.IsAccepted
            };

            return countdown;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Query to get all rides where IsAccepted is not "None"
                var acceptedFilter = TableQuery.GenerateFilterCondition("IsAccepted", QueryComparisons.NotEqual, "None");
                var notFinishedFilter = TableQuery.GenerateFilterConditionForInt("IsFinished", QueryComparisons.Equal, 0);

                var combinedFilter = TableQuery.CombineFilters(acceptedFilter, TableOperators.And, notFinishedFilter);

                var query = new TableQuery<Ride>().Where(combinedFilter);

                var rides = await storageHelper2.ExecuteQueryAsync(query);

                foreach (var ride in rides)
                {
                    bool updated = false;
                    if (ride.WaitDuration > 0)
                    {
                        ride.WaitDuration--;
                        updated = true;
                    }
                    else if (ride.WaitDuration == 0 && ride.RideDuration > 0)
                    {
                        ride.RideDuration--;
                        updated = true;
                    }
                    else if (ride.RideDuration == 0 && ride.WaitDuration == 0)
                    {
                        ride.IsFinished = 1;
                        updated = true;

                        // Retrieve and update the customer and user entities
                        User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", ride.IsAccepted);
                        User retrievedUser = storageHelper.RetrieveEntity<User>("Users", ride.Email);

                        if (retrievedCustomer != null)
                        {
                            retrievedCustomer.DrivingStatus = 0;
                            await storageHelper.InsertOrMergeEntityAsync(retrievedCustomer);
                        }

                        if (retrievedUser != null)
                        {
                            retrievedUser.DrivingStatus = 0;
                            await storageHelper.InsertOrMergeEntityAsync(retrievedUser);
                        }
                    }

                    if (updated)
                    {
                        // Update the ride status in the table
                        await storageHelper2.InsertOrMergeEntityAsync(ride);
                    }
                }

                // Delay for 1 second before next iteration
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }




        public async Task<ProfileDTO> GetProfileDTObyEmail(string email)
        {
            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);

            var user = _mapper.Map<ProfileDTO>(retrievedCustomer);

            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                return user;
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return null;
            }
        }

        public async Task<bool> UpdateProfile(ProfileDTO profile)
        {
            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", profile.Email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                if (retrievedCustomer.CheckDrivingStatus(retrievedCustomer.DrivingStatus) == "IsDriving")
                    return false;
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return false;
            }

            if (retrievedCustomer != null)
            {
                retrievedCustomer.Username = profile.Username;
                retrievedCustomer.Name = profile.Name;
                retrievedCustomer.LastName = profile.LastName;
                retrievedCustomer.DateOfBirth = profile.DateOfBirth;
                retrievedCustomer.Address = profile.Address;

                storageHelper.InsertOrMergeEntity(retrievedCustomer);
                ServiceEventSource.Current.ServiceMessage(this.Context, "User profile updated.");
                return true;
            }
            return false;
        }

        public async Task<string> ConfirmRide(string email, RideResponseDTO rideConfirmation)
        {
            Ride newRide = new Ride(email, rideConfirmation.StartAdress, rideConfirmation.EndAdress, rideConfirmation.Duration, rideConfirmation.Price);
            storageHelper2.InsertEntity(newRide);
            ServiceEventSource.Current.ServiceMessage(this.Context, "Ride entity inserted.");



            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer.DrivingStatus == 1)
                return null;

            retrievedCustomer.DrivingStatus = 1;
            storageHelper.InsertOrMergeEntity(retrievedCustomer);
            return "Ride entity inserted.D";
        }

        public async Task<List<RideTableDTO>> GetRideHistory(string email)
        {
            // Query your storage to get the ride history for the given email
            var query = new TableQuery<Ride>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Drives"))
                                              .Where(TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            var rideEntities = await storageHelper2.ExecuteQueryAsync(query);

            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                if (retrievedCustomer.CheckDrivingStatus(retrievedCustomer.DrivingStatus) == "IsDriving")
                    return null;
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return null;
            }

            // Use AutoMapper to map the list of Ride entities to a list of RideTableDTO
            var rideHistoryDTO = _mapper.Map<List<RideTableDTO>>(rideEntities);

            return rideHistoryDTO;
        }

        public async Task<List<RideTableDTO>> GetAvailableRides()
        {
            // Query your storage to get the ride history for the given email
            var query = new TableQuery<Ride>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Drives"))
                                              .Where(TableQuery.GenerateFilterCondition("IsAccepted", QueryComparisons.Equal, "None"));

            var rideEntities = await storageHelper2.ExecuteQueryAsync(query);

            // Use AutoMapper to map the list of Ride entities to a list of RideTableDTO
            var rideHistoryDTO = _mapper.Map<List<RideTableDTO>>(rideEntities);

            return rideHistoryDTO;
        }

        public async Task<bool> AcceptRide(string email, int rideId)
        {
            // Convert rideId to string because RowKey in Azure Table Storage is a string
            string rideIdString = rideId.ToString();

            // Query the storage to find the ride by its ID
            var query = new TableQuery<Ride>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rideIdString));

            var rideEntities = await storageHelper2.ExecuteQueryAsync(query);


            // Check if the ride exists and is not already accepted
            var rideEntity = rideEntities.FirstOrDefault();
            if (rideEntity == null || rideEntity.IsAccepted != "None")
            {
                return false;
            }

            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                if (retrievedCustomer.CheckDrivingStatus(retrievedCustomer.DrivingStatus) == "IsDriving")
                    return false;
                else
                    retrievedCustomer.DrivingStatus = 1;
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return false;
            }

            // Update the ride to mark it as accepted
            rideEntity.IsAccepted = email;
            
            storageHelper2.InsertOrMergeEntity(rideEntity);
            storageHelper.InsertOrMergeEntity(retrievedCustomer);


            //DODATI: vozaci vozi
            //DODATI i za available rides i driver history block check
            //unblock vrv u async-u

            return true;
        }



        public async Task<List<RideTableDTO>> GetDriverRideHistory(string email)
        {
            // Query your storage to get the ride history for the given email
            var query = new TableQuery<Ride>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Drives"))
                                              .Where(TableQuery.GenerateFilterCondition("IsAccepted", QueryComparisons.Equal, email));

            var rideEntities = await storageHelper2.ExecuteQueryAsync(query);

            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                if (retrievedCustomer.CheckDrivingStatus(retrievedCustomer.DrivingStatus) == "IsDriving")
                    return null;
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return null;
            }


            // Use AutoMapper to map the list of Ride entities to a list of RideTableDTO
            var rideHistoryDTO = _mapper.Map<List<RideTableDTO>>(rideEntities);

            return rideHistoryDTO;
        }

        public async Task<List<RideTableDTO>> GetAdminRideHistory()
        {
            var query = new TableQuery<Ride>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Drives"));

            var rideEntities = await storageHelper2.ExecuteQueryAsync(query);

            // Use AutoMapper to map the list of Ride entities to a list of RideTableDTO
            var rideHistoryDTO = _mapper.Map<List<RideTableDTO>>(rideEntities);

            return rideHistoryDTO;
        }

        public async Task<List<ProfileDTO>> GetAllDrivers()
        {
            var query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Users"))
                                              .Where(TableQuery.GenerateFilterConditionForInt("UserType", QueryComparisons.Equal, 0));

            var drivers = await storageHelper.ExecuteQueryAsync(query);

            // Use AutoMapper to map the list of Ride entities to a list of RideTableDTO
            var driversDTO = _mapper.Map<List<ProfileDTO>>(drivers);

            return driversDTO;
        }

        public async Task<bool> VerifyUser(string userId)
        {
            var query = new TableQuery<User>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId));

            var rideEntities = await storageHelper.ExecuteQueryAsync(query);

            // Check if the ride exists and is not already accepted
            var rideEntity = rideEntities.FirstOrDefault();
            if (rideEntity == null || rideEntity.VerificationStatus == 1)
            {
                return false;
            }

            // Update the ride to mark it as accepted
            rideEntity.VerificationStatus = 1;
            storageHelper.InsertOrMergeEntity(rideEntity);

            return true;
        }

        public async Task<bool> DenyUser(string userId)
        {
            var query = new TableQuery<User>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId));

            var rideEntities = await storageHelper.ExecuteQueryAsync(query);

            // Check if the ride exists and is not already accepted
            var rideEntity = rideEntities.FirstOrDefault();
            if (rideEntity == null || rideEntity.VerificationStatus == 2)
            {
                return false;
            }

            rideEntity.VerificationStatus = 2;
            storageHelper.InsertOrMergeEntity(rideEntity);

            return true;
        }

        public async Task<bool> BlockUser(string userId)
        {
            var query = new TableQuery<User>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId));

            var rideEntities = await storageHelper.ExecuteQueryAsync(query);

            // Check if the ride exists and is not already accepted
            var rideEntity = rideEntities.FirstOrDefault();
            if (rideEntity == null || rideEntity.BlockingStatus == 1)
            {
                return false;
            }

            // Update the ride to mark it as accepted
            rideEntity.BlockingStatus = 1;
            storageHelper.InsertOrMergeEntity(rideEntity);

            return true;
        }

        public async Task<bool> UnblockUser(string userId)
        {
            var query = new TableQuery<User>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId));

            var rideEntities = await storageHelper.ExecuteQueryAsync(query);

            // Check if the ride exists and is not already accepted
            var rideEntity = rideEntities.FirstOrDefault();
            if (rideEntity == null || rideEntity.BlockingStatus == 0)
            {
                return false;
            }

            rideEntity.BlockingStatus = 0;
            storageHelper.InsertOrMergeEntity(rideEntity);

            return true;
        }

        public async Task<string> FetchUserVerificationStatus(string email)
        {
            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                return retrievedCustomer.CheckVerificationStatus(retrievedCustomer.VerificationStatus);
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return "";
            }
        }

        public async Task<string> FetchUserBlockingStatus(string email)
        {
            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", email);
            if (retrievedCustomer != null)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Retrieved customer: {retrievedCustomer.Email}");
                return retrievedCustomer.CheckBlockingStatus(retrievedCustomer.BlockingStatus);
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Customer not found.");
                return "";
            }
        }

        public async Task<bool> RateDriver(string isAccepted, int rating)
        {
            User retrievedCustomer = storageHelper.RetrieveEntity<User>("Users", isAccepted);

            if (retrievedCustomer == null)
            {
                return false;
            }

            // Update rating and count
            retrievedCustomer.RatingCount += 1;
            retrievedCustomer.Rating = ((retrievedCustomer.Rating * (retrievedCustomer.RatingCount - 1)) + rating) / retrievedCustomer.RatingCount;

            // Ensure the rating is within valid bounds
            if (retrievedCustomer.Rating > 5 || retrievedCustomer.Rating < 0)
            {
                return false;
            }

            storageHelper.InsertOrMergeEntity(retrievedCustomer);

            return true;
        }
    }

}

