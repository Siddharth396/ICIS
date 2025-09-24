/* eslint-disable max-len */
module.exports = {
  async up(db, client) {
    console.log('Running migration 20250312102119-set-price-series-ids-series-item-type-code-in-cb-null');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('content_block_definitions').updateMany(
          {},
          [{
            $set: {
              price_series_ids: null,
              series_item_type_code: null,
            },
          }],
          { session },
        );

        await db.collection('user_preferences').updateMany(
          {},
          [{
            $set: {
              columns: null,
              price_series: null,
            },
          }],
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
