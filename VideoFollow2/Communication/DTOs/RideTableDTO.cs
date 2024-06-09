using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    [DataContract]
    public class RideTableDTO
    {
        [DataMember]
        public int RideDuration { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string StartAdress { get; set; }

        [DataMember]
        public string EndAdress { get; set; }

        [DataMember]
        public int WaitDuration { get; set; }

        [DataMember]
        public int Price { get; set; }

        [DataMember]
        public string IsAccepted { get; set; }
    }
}
