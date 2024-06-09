using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    [DataContract]
    public class RideRequestDTO
    {

        [DataMember]
        public string StartAdress { get; set; }

        [DataMember]
        public string EndAdress { get; set; }
    }
}
