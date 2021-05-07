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

        //returns all users images in list
        public async Task<List<Image>> GetCollection(string user_id) =>
            await _images.Find<Image>(image => image.User_Id == user_id).ToListAsync();
        //returns a specific image for the user
        public async Task<Image> Get(string id, string user_id) =>
            await _images.Find<Image>(image => image._id == id && image.User_Id == user_id).FirstOrDefaultAsync();
        //removes a users specific image
        public async Task Remove(string id, string user_id) =>
            await _images.DeleteOneAsync(image => image._id == id && image.User_Id == user_id);
        //returns the number of existing images for a specific filename
        //used for verifying if exists or not
        public async Task<long> CheckNameExists(string name, string user_id)
        {
            return await _images.CountDocumentsAsync(image => image.Name == name && image.User_Id == user_id);
            
        }
        //creates an image
        public async Task<Image> Create(Image image)
        {
            await _images.InsertOneAsync(image);
            return image;
        }

    }
}
