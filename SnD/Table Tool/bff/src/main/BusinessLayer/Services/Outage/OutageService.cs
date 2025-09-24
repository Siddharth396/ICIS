namespace BusinessLayer.Services.Outage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Services.Common;
    using BusinessLayer.Services.Models;

    using Infrastructure.SQLDB.Models;
    using Infrastructure.SQLDB.Repositories;

    using Microsoft.Data.SqlClient;

    using Serilog;

    public class OutageService : IOutageService
    {
        private readonly IGenericRepository<Outage> genericRepository;
        private readonly ILogger logger;
        private readonly ISqlQueryReader sqlQueryReader;

        public OutageService(IGenericRepository<Outage> genericRepository, ILogger logger, ISqlQueryReader sqlQueryReader)
        {
            this.genericRepository = genericRepository;
            this.logger = logger;
            this.sqlQueryReader = sqlQueryReader;
        }

        public async Task<Response<IEnumerable<Outage>>> GetOutagesByCommoditiesAndRegions(string commodities, string regions, int? pageNo, int? pageSize)
        {
            if (string.IsNullOrEmpty(commodities) || string.IsNullOrEmpty(regions))
            {
                logger.Error("Incorrect request parameter. Commodities and Region cannot be null or empty.");
                return Response<IEnumerable<Outage>>.Failure("Incorrect request parameter. Commodities and Region cannot be null or empty.", "400");
            }
            if (pageNo <= 0 || pageSize <= 0)
            {
                logger.Error("Incorrect request parameter.PageNo and PageSize both should be either null or greater than zero.");
                return Response<IEnumerable<Outage>>.Failure("Incorrect request parameter.PageNo and PageSize both should be either null or greater than zero.", "400");
            }

            string queryText;
            try
            {
                queryText = sqlQueryReader.ReadQuery("TableTool1.sql");
            }
            catch (FileNotFoundException ex)
            {
                logger.Error(ex, "Outage script doesn't exist.");
                return Response<IEnumerable<Outage>>.Failure("Some error occured while executing script.", "500");
            }
                  
            var parameters = new List<SqlParameter>
            {
                new("@products", commodities),
                new("@regions", regions)
            };

            var outages = await genericRepository.ExecuteReadOnlySql(queryText, parameters, pageNo, pageSize);

            return Response<IEnumerable<Outage>>.Success(outages);
        }
    }
}
