namespace BusinessLayer.Services.Outage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Models;

    using global::Infrastructure.SQLDB.Models;

    public interface IOutageService
    {
        Task<Response<IEnumerable<Outage>>> GetOutagesByCommoditiesAndRegions(string commodities, string regions, int? pageId, int? pageSize);
    }
}
