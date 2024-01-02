using AutoMapper;
using BusinessObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Repository
{
    public class MongoCRUD
    {
        private IMongoDatabase _db;
        public MongoCRUD(string database)
        {
            var clientt = new MongoClient("mongodb+srv://trinhtuan:123@cluster0.l8vyawf.mongodb.net/?retryWrites=true&w=majority");
            _db = clientt.GetDatabase(database);
        }
        public void InsertRecord<T>(string table, T record) where T : class
        {
            var collection = _db.GetCollection<T>(table);
            collection.InsertOne(record);
        }
        public List<T> LoadRecord<T>(string table) where T : class
        {
            var collection = _db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }
        public T LoadRecordById<T>(string table, Guid id) where T : class
        {
            var collection = _db.GetCollection<T>(table);
            // Build query to filter
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.Find(filter).First();
        }
        public void UpdateRecord<T>(string table, Guid id, T record) where T : class
        {
            var collection = _db.GetCollection<T>(table);
            var result = collection.ReplaceOne(new BsonDocument("_id", id), record, new UpdateOptions { IsUpsert = true });

        }
        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
        public T LoadRecordByField<T>(string table, string fieldNames)
        {
            var collection = _db.GetCollection<T>(table);
            string[] fields = fieldNames.Split(',', StringSplitOptions.TrimEntries);

            // Combine filters using logical AND
            var filterDefinition = Builders<T>.Filter.Empty;

            foreach (var field in fields)
            {
                string[] values = field.Split(':', StringSplitOptions.TrimEntries);

                // Update filterDefinition with an additional equality condition
                filterDefinition &= Builders<T>.Filter.Eq(values[0], values[1]);
            }

            return collection.Find(filterDefinition).FirstOrDefault();
        }

    }
    public class Config
    {
        public static void UserCreateMap(IMapperConfigurationExpression cfg)
        {
            //config transfer from User to UserDto 
            cfg.CreateMap(typeof(User), typeof(UserDTO));
            //config transfer from UserDto to User 
            cfg.CreateMap(typeof(UserDTO), typeof(User));
        }
    }
}
