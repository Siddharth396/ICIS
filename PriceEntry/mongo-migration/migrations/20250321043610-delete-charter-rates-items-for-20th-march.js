module.exports = {
  async up(db, client) {
    console.log('Running migration 20250321043610-delete-charter-rates-items-for-20th-march');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const lng = 'fa627c9b-3d8d-4ff5-b9d9-1abca5049d5f';
        const regionWorld = 'fdd61dcc-5143-42d8-9404-cbba2b60824c';
        const seriesItemTypeCode = 'cri-single';
        const appliesFrom = '2025-03-20T00:00:00.000+00:00';

        const seriesAggregationResult = await db.collection('price_series').aggregate([
          {
            $match: {
              $and: [
                { 'commodity.guid': { $eq: lng } },
                { 'location.region.guid': { $eq: regionWorld } },
                { series_item_type_code: { $eq: seriesItemTypeCode } },
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

        // Find all series items that belong to the series
        const seriesItemsAggregationResult = await db.collection('price_series_items').find({
          series_id: { $in: seriesIds },
          applies_from_datetime: { $eq: new Date(appliesFrom) },
        }, { session }).toArray();

        const seriesItemIds = seriesItemsAggregationResult.map((doc) => doc._id);

        await db.collection('price_series_items').deleteMany({
          _id: { $in: seriesItemIds },
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
