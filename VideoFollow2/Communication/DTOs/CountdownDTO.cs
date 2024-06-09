using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.DTOs
{
    public class CountdownDTO
    {
        public int WaitDuration { get; set; }

        public string IsAccepted { get; set; }
        public int RideDuration { get; set; }
    }
}
