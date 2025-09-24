module.exports = {
  async up(db, client) {
    console.log('Running migration 20241028125826-cleanup-melamine-and-styrene-data');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const melamine = '9261bd6e-bfe6-4d5f-805d-2966bd41a06b';
        const styrene = '5470f074-6fb7-48a1-aa13-18cbf3f99686';

        // Find all series that have melamine or styrene as commodity
        const seriesAggregationResult = await db.collection('price_series').aggregate([
          {
            $match: {
              'commodity.guid': { $in: [melamine, styrene] },
            },
          },
          {
            $project: {
              _id: 1,
            },
          },
        ], { session }).toArray();
        const seriesIds = seriesAggregationResult.map((doc) => doc._id);

        // Find all series items that belong to the series
        const seriesItemsAggregationResult = await db.collection('price_series_items').find({
          series_id: { $in: seriesIds },
        }, { session }).toArray();
        const seriesItemIds = seriesItemsAggregationResult.map((doc) => doc._id);

        // Find all content packages that have the series items
        const contentPackagesAggregationResult = await db.collection('content_packages').find({
          price_series_item_ids: { $in: seriesItemIds },
        }, { session }).toArray();
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
