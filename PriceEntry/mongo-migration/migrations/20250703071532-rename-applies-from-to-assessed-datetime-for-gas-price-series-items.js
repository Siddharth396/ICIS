module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703071532-rename-applies-from-to-assessed-datetime-for-gas-price-series-items');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('gas_price_series_items').updateMany(
          { applies_from_datetime: { $exists: true } },
          { $rename: { applies_from_datetime: 'assessed_datetime' } },
          { session }
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
