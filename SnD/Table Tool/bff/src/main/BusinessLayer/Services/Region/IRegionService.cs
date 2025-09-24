namespace BusinessLayer.Services.Region
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Models;

    using Infrastructure.SQLDB.Models;

    public interface IRegionService
    {
        Task<Response<IEnumerable<RegionCC>>> GetRegion();
    }
}
