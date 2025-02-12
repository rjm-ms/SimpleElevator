using SimpleElevator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleElevator.Models
{
    public class RequestBase
    {
        public int Floor { get; set; }
        public Direction Direction { get; set; }
    }
}
