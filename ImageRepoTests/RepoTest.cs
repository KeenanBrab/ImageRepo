using ImageRepo.Models;
using ImageRepo.Services;
using ImageRepoTests.Fixtures;
using Microsoft.Extensions.Configuration;
using System;
using Tynamix.ObjectFiller;
using Xunit;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ImageRepoTests
{
    public class RepoTest : IDisposable
    {

        private Filler<Image> testdatafiller = new Filler<Image>();

        public DatabaseSettings databaseSetting = new DatabaseSettings();

        public RepoTest()
        {
            
            var connString = "mongodb+srv://ImageAdmin:fcSGcVEZlSttSmqo@cluster0.t6wbq.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";

            databaseSetting.ConnectionString = connString;
            databaseSetting.DatabaseName = "TestDB";
            testdatafiller.Setup().OnProperty(x => x._id).IgnoreIt();
        }

        private Image generate()
        {
            var result = testdatafiller.Create();
            return result;
        }
        
        [Fact]
        public async Task CreateAndGetTest()
        {
            var repo = new ImageService(databaseSetting);
            var record = generate();
            var id = ObjectId.GenerateNewId().ToString();
            var user = record.User_Id;
            record._id = id;
            await repo.Create(record);
            var testrecord = await repo.Get(id, user);
            Assert.Equal(record._id, testrecord._id);
        }
        [Fact]
        public async Task RemoveAndCheckTest()
        {
            var repo = new ImageService(databaseSetting);
            var record = generate();
            var id = ObjectId.GenerateNewId().ToString();
            var user = record.User_Id;
            record._id = id;
            await repo.Create(record);
            await repo.Remove(id,user);
            var testrecord = await repo.Get(id, user);
            Assert.Null(testrecord);

        }

        [Fact]
        public async Task CheckListIsUsers()
        {
            var repo = new ImageService(databaseSetting);
            var user = "Keenan";
            
            for (int i = 0; i < 10; i++)
            {
                var record = generate();
                var id = ObjectId.GenerateNewId().ToString();
                record._id = id;
                record.User_Id = user;
                await repo.Create(record);
            }

            var images = await repo.GetCollection(user);

            foreach (Image img in images)
            {
                Assert.Equal(img.User_Id, user);
            }

            Assert.Equal(10, images.Count);

        }

        //this function checks for the number of names in the db
        //the controller calls CheckNameExists and if the value is greater then 0 it will not insert
        [Fact]
        public async Task DuplicateNameCheckTest()
        {
            var repo = new ImageService(databaseSetting);
            var user = "Keenan";
            var name = "test";
            
            var record = generate();
            var id = ObjectId.GenerateNewId().ToString();
            record._id = id;
            record.Name = name;
            record.User_Id = user;

            await repo.Create(record);
            record._id = ObjectId.GenerateNewId().ToString();
            await repo.Create(record);

            var test = await repo.CheckNameExists(name, user);

            Assert.Equal(2, test);


        }

        public void Dispose()
        {
            var connString = "mongodb+srv://ImageAdmin:fcSGcVEZlSttSmqo@cluster0.t6wbq.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";

            var client = new MongoClient(connString);
            client.DropDatabase("TestDB");

        }
    }
}
