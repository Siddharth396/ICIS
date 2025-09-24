namespace BusinessLayer.Services.Capacity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Models;

    using global::Infrastructure.SQLDB.Models;

    public interface ICapacityDevelopmentService
    {
        Task<Response<IEnumerable<CapacityDevelopment>>> GetCapacityDevelopmentsByCommoditiesAndRegions(string commodities, string regions, int? pageNo, int? pageSize);
    }
}