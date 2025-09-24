module.exports = {
  async up(db, client) {
    console.log('Running migration 20250120121506-LNG-methodology-change-over');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const lng = 'fa627c9b-3d8d-4ff5-b9d9-1abca5049d5f';
        const regionAmericas = '819d5022-a2d8-41b2-8b1c-b48e368e1766';
        const regionEurope = '636bd012-eadf-4c22-a4d7-5578de437ec6';
        const regionWorld = 'fdd61dcc-5143-42d8-9404-cbba2b60824c';

        const turkeyLocationId = '169295db-f032-40ae-a37a-fc8efbb197f1';

        const appliesFrom = '2010-01-01T00:00:00.000+00:00';
        const appliesUntil = '2025-03-20T00:00:00.000+00:00';

        // Find all series where
        // 1.commodity is lng and region is Americas,europe or world region
        // 2.Or location is Turkey
        const seriesAggregationResult = await db.collection('price_series').aggregate([
          {
            $match: {
              $or: [
                {
                  $and: [
                    { 'commodity.guid': { $eq: lng } },
                    { 'location.region.guid': { $in: [regionAmericas, regionEurope, regionWorld] } },
                  ],
                },
                {
                  'location.guid': { $eq: turkeyLocationId },
                },
              ],
            },
          },
          {
            $project: {
              _id: 1,
            },
          },
        ], { session }).toArray();

        const seriesIds = seriesAggregationResult.map((doc) => doc._id);

        // Find all content block that belong to the series
        const contentBlocksAggregationResult = await db.collection('content_block_definitions').find({
          price_series_ids: { $in: seriesIds },
        }, { session }).toArray();

        const contentBlockIds = contentBlocksAggregationResult.map((doc) => doc.content_block_id);

        // Find all series items that belong to the series
        const seriesItemsAggregationResult = await db.collection('price_series_items').find({
          series_id: { $in: seriesIds },
          applies_from_datetime: { $gte: new Date(appliesFrom), $lt: new Date(appliesUntil) },
          $or: [
            { dlh_record_source: 'SS' },
            { dlh_record_source: 'dp-analytics-platform-prod' },
            { dlh_record_source: null },
          ],
        }, { session }).toArray();

        const seriesItemIds = seriesItemsAggregationResult.map((doc) => doc._id);

        // Find all content packages that have the series items
        const contentPackagesAggregationResult = await db.collection('content_packages').find({
          price_series_item_ids: { $in: seriesItemIds },
        }, { session }).toArray();

        const contentPackageIds = contentPackagesAggregationResult.map((doc) => doc._id);

        // Delete all price_series_items, content_packages and pricing_content_packages
        // & commentaries

        await db.collection('price_series_items').deleteMany({
          _id: { $in: seriesItemIds },
        }, { session });

        await db.collection('content_packages').deleteMany({
          _id: { $in: contentPackageIds },
        }, { session });

        await db.collection('pricing_content_packages').deleteMany({
          _id: { $in: contentPackageIds },
        }, { session });

        await db.collection('commentaries').deleteMany({
          content_block_id: { $in: contentBlockIds },
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
