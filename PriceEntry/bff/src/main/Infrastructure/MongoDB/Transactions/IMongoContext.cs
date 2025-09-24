namespace Infrastructure.MongoDB.Transactions
{
    using global::MongoDB.Driver;

    public interface IMongoContext
    {
        IClientSessionHandle Session { get; }
    }
}
