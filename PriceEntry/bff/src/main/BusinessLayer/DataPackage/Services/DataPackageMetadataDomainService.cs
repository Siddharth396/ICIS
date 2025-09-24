namespace BusinessLayer.DataPackage.Services
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Handler;
    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories;
    using BusinessLayer.DataPackage.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.EventDispatcher;

    using Microsoft.Extensions.Caching.Memory;

    [DomainService]
    public class DataPackageMetadataDomainService : IDataPackageMetadataDomainService
    {
        private readonly DataPackageMetadataRepository dataPackageMetadataRepository;
        private readonly IEventDispatcher dispatcher;
        private readonly DataPackageIdCache dataPackageIdCache;

        public DataPackageMetadataDomainService(
            DataPackageMetadataRepository dataPackageMetadataRepository,
            IEventDispatcher dispatcher,
            IMemoryCache memoryCache)
        {
            this.dataPackageMetadataRepository = dataPackageMetadataRepository;
            this.dispatcher = dispatcher;
            this.dataPackageIdCache = new DataPackageIdCache(memoryCache);
        }

        public async Task<DataPackageId> GetDataPackageId(DataPackageKey dataPackageKey)
        {
            var cachedDataPackageId = dataPackageIdCache.Get(dataPackageKey);

            if (cachedDataPackageId != null)
            {
                return cachedDataPackageId;
            }

            var metadata = await GetOrSaveDataPackageMetadata(dataPackageKey);
            var dataPackageId = new DataPackageId(metadata.Id);

            dataPackageIdCache.Set(dataPackageKey, dataPackageId);

            return dataPackageId;
        }

        public async Task ToggleNonMarketAdjustment(DataPackageKey dataPackageKey, bool enabled)
        {
            var metadata = await GetOrSaveDataPackageMetadata(dataPackageKey);
            metadata.NonMarketAdjustmentEnabled = enabled;
            await dataPackageMetadataRepository.Save(metadata);

            if (enabled == false)
            {
                await dispatcher.DispatchAsync(
                    new NonMarketAdjustmentDisabledEvent(
                        dataPackageKey.ContentBlockId,
                        dataPackageKey.ContentBlockVersion,
                        dataPackageKey.AssessedDateTime));
            }
        }

        public async Task<bool> IsNonMarketAdjustmentEnabled(DataPackageKey dataPackageKey)
        {
            var metadata = await GetOrSaveDataPackageMetadata(dataPackageKey);
            return metadata.NonMarketAdjustmentEnabled;
        }

        private async Task<DataPackageMetadata> GetOrSaveDataPackageMetadata(DataPackageKey dataPackageKey)
        {
            var metadata = await dataPackageMetadataRepository.GetBy(dataPackageKey);
            if (metadata == null)
            {
                metadata = new DataPackageMetadata
                {
                    Id = Guid.NewGuid().ToString(),
                    DataPackageIdMetadata = new DataPackageIdMetadata
                    {
                        AssessedDateTime = dataPackageKey.AssessedDateTime,
                        ContentBlockId = dataPackageKey.ContentBlockId,
                        ContentBlockVersion = dataPackageKey.ContentBlockVersion.Value
                    }
                };

                await dataPackageMetadataRepository.Save(metadata);
            }

            return metadata;
        }

        private class DataPackageIdCache
        {
            private readonly IMemoryCache cache;

            public DataPackageIdCache(IMemoryCache cache)
            {
                this.cache = cache;
            }

            public DataPackageId? Get(DataPackageKey dataPackageKey)
            {
                return cache.Get<DataPackageId>(dataPackageKey);
            }

            public void Set(DataPackageKey dataPackageKey, DataPackageId dataPackageId)
            {
                cache.Set(
                    dataPackageKey,
                    dataPackageId,
                    new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    });
            }
        }
    }
}
