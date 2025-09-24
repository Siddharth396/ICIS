module.exports = {
  async up(db, client) {
    console.log('Running migration 20240729083503-add-price-series-items-index');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('price_series_items')
          .createIndex(
            {
              series_id: 1,
              applies_from_datetime: 1,
            },
            {
              unique: true,
            },
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
