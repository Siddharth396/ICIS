namespace Infrastructure.MongoDB.Repositories
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;

    using Infrastructure.MongoDB.Transactions;

    [ExcludeFromCodeCoverage]
    public abstract class BaseRepository<T>
    {
        protected BaseRepository(IMongoDatabase database, IMongoContext mongoContext)
        {
            Collection = database.GetCollection<T>(DbCollectionName);
            Context = mongoContext;
        }

        public abstract string DbCollectionName { get; }

        protected IMongoContext Context { get; }

        private IMongoCollection<T> Collection { get; }

        protected IAsyncCursor<T> Aggregate(PipelineDefinition<T, T> pipeline)
        {
            return Collection.Aggregate(Context.Session, pipeline);
        }

        protected Task InsertOneAsync(
            T document,
            InsertOneOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return Collection.InsertOneAsync(
                Context.Session,
                document,
                options,
                cancellationToken);
        }

        protected virtual IMongoQueryable<T> Query()
        {
            return Collection.AsQueryable(Context.Session);
        }

        protected virtual IMongoQueryable<TK> QueryOfType<TK>()
            where TK : T
        {
            return Collection.OfType<TK>().AsQueryable(Context.Session);
        }

        protected Task<ReplaceOneResult> ReplaceOneAsync(
            FilterDefinition<T> filter,
            T replacement,
            ReplaceOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return Collection.ReplaceOneAsync(
                Context.Session,
                filter,
                replacement,
                options,
                cancellationToken);
        }

        protected Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition)
        {
            return Collection.UpdateManyAsync(
                Context.Session,
                filter,
                updateDefinition);
        }

        protected Task UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions? options = null)
        {
            return Collection.UpdateOneAsync(
                Context.Session,
                filter,
                update,
                options);
        }

        protected Task DeleteOneAsync(FilterDefinition<T> filter)
        {
            return Collection.DeleteOneAsync(
                Context.Session,
                filter);
        }

        protected Task UpdateBulkAsync(IEnumerable<WriteModel<T>> writeModels)
        {
            return Collection.BulkWriteAsync(Context.Session, writeModels);
        }
    }
}
