using ImageRepo.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepo.Services
{
    public class ImageService
    {
        private readonly IMongoCollection<Image> _images;

        public ImageService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _images = database.GetCollection<Image>("Images");

        }

        public List<Image> GetCollection(string user_id) =>
            _images.Find<Image>(image => image.User_Id == user_id).ToList();

        public Image Get(MongoDB.Bson.ObjectId id) =>
            _images.Find<Image>(image => image._id == id).FirstOrDefault();

        public Image Create(Image image)
        {
            _images.InsertOne(image);
            return image;
        }
    }
}
