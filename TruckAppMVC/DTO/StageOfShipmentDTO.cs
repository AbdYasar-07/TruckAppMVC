using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TruckAppMVC.DTO
{
    public class StageOfShipmentDTO
    {
        public string _id { get; set; }
        public string ReferenceNo { get; set; }
        public string Stage { get; set; }
        public string IsCompleted { get; set; }
        public string Comments { get; set; }
    }
}
