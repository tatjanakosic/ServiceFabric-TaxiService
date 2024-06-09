using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [DataContract]
    public class Ride : TableEntity
    {
        public Ride()
        {
        }

        public Ride(string email, string startAdress, string endAdress, int duration, int price)
        {
            PartitionKey = "Drives";
            Id = GenerateUnique();
            RowKey = Id.ToString();
            Email = email;
            StartAdress = startAdress;
            EndAdress = endAdress;
            WaitDuration = duration;
            RideDuration = GenerateUnique() % 100;
            Price = price;
            IsAccepted = "None";
            IsFinished = 0;
        }

        public int GenerateUnique()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int uniqueNumber = (int)(timestamp % 1000000);

            return uniqueNumber;
        }

        [DataMember]
        public int RideDuration { get; set; }

        [DataMember]
        public int Id {  get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string StartAdress {  get; set; }

        [DataMember]
        public string EndAdress {  get; set; }

        [DataMember]
        public int WaitDuration { get; set; }

        [DataMember]
        public int Price { get; set; }

        [DataMember]
        public string IsAccepted { get; set; }

        [DataMember]
        public int IsFinished { get; set; }

    }
}
