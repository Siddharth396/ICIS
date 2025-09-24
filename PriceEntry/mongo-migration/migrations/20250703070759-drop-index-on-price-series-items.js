module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703070759-drop-index-on-price-series-items');
     const session = client.startSession();
    try {
       await session.withTransaction(async () => {
        await db.collection('price_series_items').dropIndex({ series_id: 1, applies_from_datetime: 1 });
       });
    } finally {
       await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
