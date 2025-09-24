module.exports = {
  async up(db, client) {
    console.log('Running migration 20250122113953-add-versionId-field');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('price_series_items').updateMany(
          { _id: { $ne: null } },
          [
            {
              $set: { series_item_id: '$_id' },
            },
          ],
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
