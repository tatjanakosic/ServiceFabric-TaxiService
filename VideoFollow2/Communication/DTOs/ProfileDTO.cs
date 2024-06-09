using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    [DataContract]
    public class ProfileDTO
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public int VerificationStatus { get; set; }

        [DataMember]
        public int BlockingStatus { get; set; }

        [DataMember]
        public double Rating { get; set; }

    }
}
