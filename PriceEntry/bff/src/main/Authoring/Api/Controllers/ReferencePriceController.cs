namespace Authoring.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference;

    using Microsoft.AspNetCore.Mvc;

    using Serilog;
    using Serilog.Context;

    [Route("v1/[controller]")]
    [ApiController]
    public class ReferencePriceController : ControllerBase
    {
        private readonly ILogger logger;

        private readonly IReferencePriceService referencePriceService;

        public ReferencePriceController(
                       ILogger logger,
                       IReferencePriceService referencePriceService)
        {
            this.logger = logger.ForContext<ReferencePriceController>();
            this.referencePriceService = referencePriceService;
        }

        [HttpPost]
        [Route("gas/published")]
        public async Task<List<string>> GasPricePublished([FromBody] GasPricePayload input)
        {
            using (LogContext.PushProperty("Flow", "GasPricePublished"))
            {
                var localLogger = logger.ForContext("MarketCode", input.MarketCode)
                    .ForContext("Payload", input, true);

                localLogger.Information("Notification received about gas reference price being published.");

                var result = await referencePriceService.UpdateGasReferencePrice(input);

                localLogger.Information("Reference price has been updated based on published gas price.");

                return result;
            }
        }
    }
}
