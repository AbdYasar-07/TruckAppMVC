using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TruckAppMVC.Model
{
    public class FaceBookTracker
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement]
        public string ReferenceNo { get; set; }
        [BsonElement]
        public DateTime DelDate { get; set; }

    }
}
