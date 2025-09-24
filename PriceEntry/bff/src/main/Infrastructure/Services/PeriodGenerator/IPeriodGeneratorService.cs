namespace Infrastructure.Services.PeriodGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPeriodGeneratorService
    {
        Task<PeriodGeneratorOutput> GeneratePeriods(DateOnly referenceDate, List<string> fulfilmentPeriodCodes);

        Task<PublicationScheduleOutput> GetPublicationSchedules(PublicationScheduleInput input);
    }
}
