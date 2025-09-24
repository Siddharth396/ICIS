namespace Test.Infrastructure.Mongo.Repositories
{
    using System.Threading.Tasks;

    using MongoDB.Driver;

    public class Repository<T>
       where T : class
    {
        private readonly IMongoCollection<T> collection;

        public Repository(IMongoDatabase database, string collectionName)
        {
            collection = database.GetCollection<T>(collectionName);
        }

        public Task InsertOneAsync(T document)
        {
            return collection.InsertOneAsync(document);
        }
    }
}
