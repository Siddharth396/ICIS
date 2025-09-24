namespace BusinessLayer.Services.Region
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Models;

    using Infrastructure.SQLDB.Models;
    using Infrastructure.SQLDB.Repositories;

    using Serilog;

    public class RegionService : IRegionService
    {
        IGenericRepository<RegionCC> genericRepository;
        private ILogger logger;
        public RegionService(IGenericRepository<RegionCC> genericRepository, ILogger logger)
        {
            this.genericRepository = genericRepository;
            this.logger = logger;
        }
        public async Task<Response<IEnumerable<RegionCC>>> GetRegion()
        {
            var res = genericRepository.GetQueryable();
            var regions = res.Where(x=>x.Description.StartsWith("Order_")).Select(x=>new RegionCC {Id=x.Id,Code=x.Code.Replace("Od",""),Description=x.Description.Replace("Order_","") }).ToList();
            return Response<IEnumerable<RegionCC>>.Success(regions);

        }
    }
}
