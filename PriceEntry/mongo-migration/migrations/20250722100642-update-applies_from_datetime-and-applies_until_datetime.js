const { dateRangeFromLabel } = require('../helpers/dateFunctions');

module.exports = {
  async up(db, client) {
    console.log(
      'Running migration 20250722100642-update-applies_from_datetime-and-applies_until_datetime.js'
    );

    const session = client.startSession();

    try {
      await session.withTransaction(async () => {
        // Step 1: Find all relevant series_ids from price_series
        const priceSeriesFilteredData = await db.collection('price_series').find(
          { 
            period_label_type_code: 'plt-ref-time' 
          },
          { projection: { 
            _id: 1 
            } 
          },
          { session }
        ).toArray();

        const seriesIds = priceSeriesFilteredData.map((s) => s._id);

        // Step 2: Fetch price series items to update
        const priceSeriesItems = await db.collection('price_series_items')
          .find(
            {
              series_id: { $in: seriesIds },
              period_label: { $exists: true, $ne: null }
            },
            {
              projection: {
                _id: 1,
                period_label: 1
              }
            },
            { session }
          )
          .sort({ assessed_datetime: -1 })
          .toArray();

        // Step 3: Update documents individually
        await Promise.all(
          priceSeriesItems.map(async (item) => {
            const { appliesFrom, appliesUntil } = dateRangeFromLabel(item.period_label);

            await db.collection('price_series_items').updateOne(
              { _id: item._id },
              {
                $set: {
                  applies_from_datetime: appliesFrom,
                  applies_until_datetime: appliesUntil,
                },
              },
              { session }
            );
          })
        );
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
