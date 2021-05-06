using ImageRepo.Models;
using MongoDB.Bson;
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

        public Image Get(string id) =>
            _images.Find<Image>(image => image._id == id).FirstOrDefault();
        public async Task Remove(string id) =>
            await _images.DeleteOneAsync(image => image._id == id);
        public async Task<long> CheckNameExists(string name, string user_id)
        {
            return await _images.CountDocumentsAsync(image => image.Name == name && image.User_Id == user_id);
            
        }

        public async Task<Image> Create(Image image)
        {
            await _images.InsertOneAsync(image);
            return image;
        }
    }
}
