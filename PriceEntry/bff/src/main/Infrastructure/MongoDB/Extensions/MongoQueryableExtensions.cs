namespace Infrastructure.MongoDB.Extensions
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;

    [ExcludeFromCodeCoverage]
    public static class MongoQueryableExtensions
    {
        /// <summary>
        ///     Running LINQ statements on IMongoQueryable returns IQueryable which will crash on async hydration routines
        ///     if there is a clash with System.Linq extension methods. This re-casts to IMongoQueryable
        ///     <T>
        ///         but MUST NOT be run with IQueryables from other sources
        /// </summary>
        public static IMongoQueryable<T> AsMongoQueryable<T>(this IQueryable<T> mongoQuery)
        {
            return (IMongoQueryable<T>)mongoQuery;
        }

        /// <summary>
        ///     Running LINQ statements on IMongoQueryable returns IQueryable which will crash on ToListAsync
        ///     this enforces running the Mongo version, but MUST NOT be run with IQueryables from other sources
        /// </summary>
        public static Task<List<T>> ToMongoListAsync<T>(this IQueryable<T> mongoQuery)
        {
            return ((IMongoQueryable<T>)mongoQuery).ToListAsync();
        }
    }
}