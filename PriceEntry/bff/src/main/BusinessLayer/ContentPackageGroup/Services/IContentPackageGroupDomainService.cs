namespace BusinessLayer.ContentPackageGroup.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.ValueObjects;

    public interface IContentPackageGroupDomainService
    {
        Task SaveContentPackageGroup(string contentBlockId, Version contentBlockVersion, List<string> priceSeriesIds);

        Task<string> GetSequenceId(string contentBlockId, int version);
    }
}
