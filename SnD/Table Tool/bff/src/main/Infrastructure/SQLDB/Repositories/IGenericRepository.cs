namespace Infrastructure.SQLDB.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;

    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> ExecuteReadOnlySql(string sql, List<SqlParameter>? sqlParameters = null, int? pageNo = null, int? pageSize = null, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(int id);
        void DeleteAsync(T entity);
        void UpdateAsync(T entity);
        Task<int> InsertAsync(T entity);
    }
}
