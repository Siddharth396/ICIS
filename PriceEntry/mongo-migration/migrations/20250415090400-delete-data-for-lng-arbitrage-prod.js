module.exports = {
  async up(db, client) {
    console.log('Running migration 20250415090400-delete-data-for-lng-arbitrage-prod');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const startDate = '2024-09-02T00:00:00.000+00:00';
        const endDate = '2025-04-10T00:00:00.000+00:00';

        const seriesIds = [
          'f376ee2d-8558-4ab0-b8b8-a6e2f39604da',
          'be1496d4-64d6-459d-95db-286f1f1d5194',
          '12f2fb6b-65d8-41da-991d-c12d8499478a',
          '68918029-a8db-442f-bf8c-ae20649594ad',
          '20bc9bbb-5faa-490d-ade5-e26d7ddffb20',
        ];

        // Find all series items that belong to the series
        const seriesItemsAggregationResult = await db.collection('price_series_items').find({
          series_id: { $in: seriesIds },
          applies_from_datetime: { $gte: new Date(startDate), $lte: new Date(endDate) },
          dlh_record_source: 'lng-arbitrage-prod',
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
