using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsolatedProcess
{
    public class MongoDBCRUD : IDisposable
    {
        private IMongoDatabase db;

        public MongoDBCRUD(string database)
        {
            var client = new MongoClient();

            db = client.GetDatabase(database);
        }

        public void InsertRecord<T>(string tableName, T record)
        {
            var collection = db.GetCollection<T>(tableName);

            collection.InsertOne(record);
        }

        public List<T> LoadRecords<T>(string tableName)
        {
            var collections = db.GetCollection<T>(tableName);

            return collections.Find(new BsonDocument()).ToList();
        }

        public TResult LoadRecordById<TResult>(string tableName, Guid id)
        {
            var collection = db.GetCollection<TResult>(tableName);

            var filter = Builders<TResult>.Filter.Eq("Id", id);

            return collection.Find(filter).ToList().FirstOrDefault();
        }

        [Obsolete]
        public dynamic UpsertRecord<T>(string tableName, Guid id, T newData)
        {
            var collection = db.GetCollection<T>(tableName);

            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = collection.ReplaceOne( filter , newData , new UpdateOptions() {IsUpsert= true });

            return result;
        }


        public dynamic DeleteRecord<T>(string tablName, Guid id)
        {
            var collection = db.GetCollection<T>(tablName);

            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.DeleteOne(filter);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

}
