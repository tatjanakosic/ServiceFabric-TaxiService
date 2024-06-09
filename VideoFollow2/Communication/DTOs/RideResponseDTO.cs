using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    [DataContract]
    public class RideResponseDTO
    {
        public RideResponseDTO(string startAdress, string endAdress, int duration, int price)
        {
            StartAdress = startAdress;
            EndAdress = endAdress;
            Duration = duration;
            Price = price;
        }

        [DataMember]
        public string StartAdress { get; set; }

        [DataMember]
        public string EndAdress { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public int Price { get; set; }
    }
}
