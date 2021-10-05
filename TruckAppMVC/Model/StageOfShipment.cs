using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TruckAppMVC.Model
{
    public class StageOfShipment
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string ReferenceNo { get; set; }
        public string Stage { get; set; }
        public string IsCompleted { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }
}
