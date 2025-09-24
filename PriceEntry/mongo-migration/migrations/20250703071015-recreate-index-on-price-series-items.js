module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703071015-recreate-index-on-price-series-items');
     const session = client.startSession();
    try {
       await session.withTransaction(async () => {
        await db.collection('price_series_items').createIndex(
          {
            series_id: 1,
            assessed_datetime: 1,
          },
          {
            unique: true,
          }
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
