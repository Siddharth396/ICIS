namespace MongoDbSeeder.Repositories
{
    using System.Collections.Generic;

    using MongoDB.Driver;

    public class Repository<T>
        where T : class
    {
        private readonly IMongoCollection<T> collection;

        public Repository(IMongoDatabase database, string collectionName)
        {
            collection = database.GetCollection<T>(collectionName);
        }

        public void Insert(IEnumerable<T> documents)
        {
            collection.InsertMany(documents);
        }

        public void RemoveDocuments()
        {
            collection.DeleteMany("{}");
        }
    }
}
