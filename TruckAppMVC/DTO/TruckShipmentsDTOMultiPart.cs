using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TruckAppMVC.DTO
{
    public class TruckShipmentsDTOMultiPart
    {
        public string ReferenceNo { get; set; }
        public string Stage { get; set; }
        public string ProcessName { get; set; }
        public string Image { get; set; }
        public IFormFile ImageName { get; set; }
    }
}
