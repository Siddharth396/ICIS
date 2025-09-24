namespace Infrastructure.SQLDB.Extensions
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    public static class ReadOnlyExtensions
    {
        public static IQueryable<T> AsReadOnly<T>(this IQueryable<T> query) where T : class
        {
            return query.AsNoTracking();
        }
    }
}
