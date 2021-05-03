using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ImageRepo.Models
{
    public class InputImage
    {
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
