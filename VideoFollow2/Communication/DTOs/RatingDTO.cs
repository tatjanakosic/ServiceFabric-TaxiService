using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    [DataContract]
    public class RatingDTO
    {
        [DataMember]
        public int Rating {  get; set; }

        [DataMember]
        public string IsAccepted { get; set; }
    }
}
