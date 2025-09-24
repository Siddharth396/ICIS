namespace Test.Infrastructure.Mongo.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class GenericRepository
    {
        private readonly IMongoDatabase database;

        public GenericRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task InsertRawDocument(string collectionName, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));
            }

            try
            {
                var bsonDocument = BsonDocument.Parse(json);
                await database.GetCollection<BsonDocument>(collectionName).InsertOneAsync(bsonDocument);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to insert raw BSON document.", ex);
            }
        }

        public async Task DeleteDocumentById(string collectionName, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("ID cannot be null or empty.", nameof(id));
            }

            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var result = await database.GetCollection<BsonDocument>(collectionName).DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException($"No document found with ID: {id}");
            }
        }

        public async Task<List<T>> GetDocument<T>(string collectionName, params Expression<Func<T, bool>>[] conditions)
        {
            try
            {
                var collection = database.GetCollection<T>(collectionName);
                var filterBuilder = Builders<T>.Filter;
                var filter = filterBuilder.And(conditions.Select(filterBuilder.Where));

                return await collection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get data.", ex);
            }
        }
    }
}
