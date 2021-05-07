using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageRepo.Data;
using ImageRepo.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ImageRepoTests.Fixtures
{

        public class MongoDbFixture : IDisposable
        {
        public ApplicationDbContext DbContext { get; }
        public DatabaseSettings DatabaseSetting { get; }

        public IMongoDatabase database { get; }

        public MongoDbFixture()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables()
                .Build();
            var connString = config.GetConnectionString("ConnectionString");

            
           
            var client = new MongoClient(connString);
            database = client.GetDatabase("testDB");
            

        }
        public void Dispose()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables()
                .Build();
            var connString = config.GetConnectionString("ConnectionString");

            var client = new MongoClient(connString);
            client.DropDatabase("testDB");

        }
    }
}
