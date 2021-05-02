using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ImageRepo.Models
{
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] ContentImage { get; set; }
    }
}
