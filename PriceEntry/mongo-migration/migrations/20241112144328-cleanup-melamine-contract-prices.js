module.exports = {
  async up(db, client) {
    console.log('Running migration 20241112144328-cleanup-melamine-contract-prices');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const melamineEuropeQuarterlyContractSeriesId = '1e1a7909-9c12-4866-9b98-4b82eadd5bf3';
        const melamineEuropeMonthlyContractSeriesId = 'e2db67fc-f9f9-4934-8d5f-449adbef6bbc';
        const melamineUSQuarterlyContractSeriesId = '2a23cc01-919d-464b-8880-047bec97f934';

        const seriesIds = [
          melamineEuropeQuarterlyContractSeriesId,
          melamineEuropeMonthlyContractSeriesId,
          melamineUSQuarterlyContractSeriesId,
        ];

        // Find all series items that belong to the series
        const seriesItemsAggregationResult = await db.collection('price_series_items').find({
          series_id: { $in: seriesIds },
        }, { session }).project({ _id: 1 }).toArray();
        const seriesItemIds = seriesItemsAggregationResult.map((doc) => doc._id);

        // Find all content packages that have the series items
        const contentPackagesAggregationResult = await db.collection('content_packages').find({
          price_series_item_ids: { $in: seriesItemIds },
        }, { session }).project({ _id: 1 }).toArray();
        const contentPackageIds = contentPackagesAggregationResult.map((doc) => doc._id);

        // Delete all price_series_items, content_packages and pricing_content_packages
        await db.collection('price_series_items').deleteMany({
          _id: { $in: seriesItemIds },
        }, { session });

        await db.collection('content_packages').deleteMany({
          _id: { $in: contentPackageIds },
        }, { session });

        await db.collection('pricing_content_packages').deleteMany({
          _id: { $in: contentPackageIds },
        }, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
