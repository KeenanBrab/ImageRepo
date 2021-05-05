using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ImageRepo.Models
{
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }
        public string User_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

        public Image()
        {
            _id = ObjectId.GenerateNewId();
        }
    }
}
