namespace Infrastructure.SQLDB.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Infrastructure.SQLDB.Extensions;

    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext dbContext;
        public GenericRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbContext.Set<T>().AsReadOnly().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ExecuteReadOnlySql(string sql, List<SqlParameter>? sqlParameters = null, int? pageNo = null, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            if (pageNo.HasValue && pageSize.HasValue)
            {
                sql = string.Concat(sql, "\n OFFSET " + (pageNo.Value - 1) * pageSize.Value + " ROWS \n FETCH NEXT " + pageSize.Value + " ROWS ONLY");
            }
            var query = dbContext.Set<T>().FromSqlRaw(sql, sqlParameters?.ToArray() ?? new SqlParameter[0]);

            return await query.AsReadOnly().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public async Task<int> InsertAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
            return await dbContext.SaveChangesAsync();
        }

        public void UpdateAsync(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }
        public IQueryable<T> GetQueryable()
        {
            return dbContext.Set<T>();
        }
    }
}
