module.exports = {
  async up(db, client) {
    console.log('Running migration 20240913120317-rename-series-item-type-codes');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const updates = [
          {
            search: 'single-value',
            replace: 'pi-single',
          },
          {
            search: 'single-value-ref',
            replace: 'pi-single-with-ref',
          },
          {
            search: 'price-range',
            replace: 'pi-range',
          },
        ];

        await Promise.all(
          updates.map((update) => db.collection('content_block_definitions').updateMany(
            { series_item_type_code: update.search },
            {
              $set: {
                series_item_type_code: update.replace,
              },
            },
            { session },
          )),
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
