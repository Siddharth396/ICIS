module.exports = {
  async up(db, client) {
    console.log('Running migration 20250121092544-update-period-label-for-Irregular-period');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const periodTypeCode = 'I';
        const periodCode = 'MIRF0';

        const seriesAggregationResult = await db.collection('price_series').aggregate([
          {
            $match: {
              'relative_fulfilment_period.type.code': periodTypeCode,
              'relative_fulfilment_period.code': periodCode,
            },
          },
          {
            $project: {
              _id: 1,
            },
          },
        ], { session }).toArray();

        const seriesIds = seriesAggregationResult.map((doc) => doc._id);

        if (seriesIds.length > 0) {
          await Promise.all(seriesIds.map(async (seriesId) => {
            await db.collection('price_series_items').updateMany(
              { series_id: seriesId },
              [
                {
                  $set: {
                    period_label: {
                      $reduce: {
                        input: [
                          { find: 'January', replacement: 'Jan' },
                          { find: 'February', replacement: 'Feb' },
                          { find: 'March', replacement: 'Mar' },
                          { find: 'April', replacement: 'Apr' },
                          { find: 'May', replacement: 'May' },
                          { find: 'June', replacement: 'Jun' },
                          { find: 'July', replacement: 'Jul' },
                          { find: 'August', replacement: 'Aug' },
                          { find: 'September', replacement: 'Sep' },
                          { find: 'October', replacement: 'Oct' },
                          { find: 'November', replacement: 'Nov' },
                          { find: 'December', replacement: 'Dec' },
                        ],
                        initialValue: '$period_label',
                        in: {
                          $replaceAll: {
                            input: '$$value',
                            find: '$$this.find',
                            replacement: '$$this.replacement',
                          },
                        },
                      },
                    },
                  },
                },
              ],
              { session },
            );
          }));
        }
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
