namespace Infrastructure.MongoDB.Transactions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;

    [ExcludeFromCodeCoverage]
    internal class MongoContext : IDisposable, IMongoContext
    {
        private readonly IMongoClient mongoClient;

        private bool disposed;

        private IClientSessionHandle? session;

        public MongoContext(IMongoClient mongoClient)
        {
            this.mongoClient = mongoClient;
        }

        public IClientSessionHandle Session => session ?? throw new ArgumentNullException(nameof(session));

        public Task AbortChangesAsync()
        {
            if (session is { IsInTransaction: true })
            {
                return session.AbortTransactionAsync();
            }

            return Task.CompletedTask;
        }

        public Task CommitChangesAsync()
        {
            if (session is { IsInTransaction: true })
            {
                return session.CommitTransactionAsync();
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task StartSessionAsync()
        {
            session = await mongoClient.StartSessionAsync();

            // session.StartTransaction();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeSession();
            }

            disposed = true;
        }

        protected virtual void DisposeSession()
        {
            if (session != null)
            {
                if (session.IsInTransaction)
                {
                    session.AbortTransaction();
                }

                session.Dispose();
                session = null;
            }
        }
    }
}
