module.exports = {
  async up(db, client) {
    console.log('Running migration 20250305085759-update-series-item-type-code-sci-single');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('price_series_items').updateMany(
          { series_item_type_code: 'SCI-SINGLE' },
          {
            $set: {
              series_item_type_code: 'sci-single',
            },
          },
          { session },
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
