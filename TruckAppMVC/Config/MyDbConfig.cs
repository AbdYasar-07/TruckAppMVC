using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TruckAppMVC.Config
{
    public class MyDbConfig
    {
        public const string SectionName = "MyDbConfig";

        public string ConnectionString { get; set; }
        public string  Database { get; set; }
        public string FBT_Collection { get; set; }
        public string TS_Collection { get; set; }
        public string SOS_Collection { get; set; }
    }
}
