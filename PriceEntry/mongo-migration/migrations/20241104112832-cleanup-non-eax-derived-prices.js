module.exports = {
  async up(db, client) {
    console.log('Running migration 20241104112832-cleanup-non-eax-derived-prices');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const priceCategoryCode = 'DERIVED';
        const asiaEastLocationGuid = 'f5705134-98a1-487d-ba79-399438c34214';
        const regionalAverageDerivationFunctionKey = 'regional-avg';
        const priceDerivationTypeCode = 'INDEX';

        // Find all series that have melamine or styrene as commodity
        const seriesAggregationResult = await db.collection('price_series').aggregate([
          {
            $match: {
              'price_category.code': priceCategoryCode,
              'location.guid': { $ne: asiaEastLocationGuid },
              'derivation_inputs.derivation_function_key': regionalAverageDerivationFunctionKey,
              'price_derivation_type.code': priceDerivationTypeCode,
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
        }, { session }).project({ _id: 1 }).toArray();
        const seriesItemIds = seriesItemsAggregationResult.map((doc) => doc._id);

        // Delete all price_series_items
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
