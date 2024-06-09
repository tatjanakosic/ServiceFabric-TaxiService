using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{

    public enum BlockingStatus
    {
        Unblocked,
        Blocked
    }

    public enum VerificationStatus
    {
        Accepted,
        Pending,
        Declined
    }
    public enum UserType
    {
        Driver,
        Admin,
        User
    }

    public enum DrivingStatus
    {
        NotDriving,
        IsDriving
    }

    [DataContract]
    public class User : TableEntity
    {

        public User() { }

        public User(string email, string password, string username, string name, string lastName, DateTime dateOfBirth, string address, int userType, string image)
        {
            PartitionKey = "Users";
            RowKey = email;
            Email = email;
            Password = password;
            Username = username;
            Name = name;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Address = address;
            UserType = userType;
            Image = image;
            RatingCount = 0;
            Rating = 0;
        }

        public string CheckType(int userType)
        {
            if (userType == 0)
            {
                return Communication.UserType.Driver.ToString();
            }
            else if (userType == 1)
            {
                return Communication.UserType.Admin.ToString();
            }
            else
            {
                return Communication.UserType.User.ToString();
            }
        }

        public string CheckDrivingStatus(int drivingStatus)
        {
            if (drivingStatus == 0)
            {
                return Communication.DrivingStatus.NotDriving.ToString();
            }
            else
            {
                return Communication.DrivingStatus.IsDriving.ToString();
            }
        }

        public string CheckVerificationStatus(int verificationStatus)
        {
            if (verificationStatus == 0)
            {
                return Communication.VerificationStatus.Pending.ToString();
            }
            else if (verificationStatus == 1)
            {
                return Communication.VerificationStatus.Accepted.ToString();
            }
            else
            {
                return Communication.VerificationStatus.Declined.ToString();
            }
        }

        public string CheckBlockingStatus(int blockignStatus)
        {
            if (blockignStatus == 0)
            {
                return Communication.BlockingStatus.Unblocked.ToString();
            }
            else
            {
                return Communication.BlockingStatus.Blocked.ToString();
            }
        }

        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public int UserType { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public int VerificationStatus { get; set; }

        [DataMember]
        public int BlockingStatus { get; set; }

        [DataMember]
        public int DrivingStatus { get; set; }

        [DataMember]

        public double Rating { get; set; }

        [DataMember]
        public int RatingCount { get; set; }


    }
}
