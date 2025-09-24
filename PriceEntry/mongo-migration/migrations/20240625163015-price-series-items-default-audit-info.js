module.exports = {
  async up(db, client) {
    console.log('Running migration 20240625163015-price-series-items-default-audit-info');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('price_series_items').updateMany(
          {},
          [{
            $set: {
              last_modified: {
                timestamp: '$creation_datetime',
                user: 'migration',
              },
              created: {
                timestamp: '$creation_datetime',
                user: 'migration',
              },
            },
          }],
        );
      });

      await db.collection('price_series_items').updateMany(
        {},
        {
          $unset: { creation_datetime: '' },
        },
      );
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
