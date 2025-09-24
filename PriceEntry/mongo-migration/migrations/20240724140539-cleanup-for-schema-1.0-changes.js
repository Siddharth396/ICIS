module.exports = {
  async up(db, client) {
    console.log('Running migration 20240724140539-cleanup-for-schema-1.0-changes');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('user_preferences').deleteMany({});
        await db.collection('commentaries').deleteMany({});
        await db.collection('content_block_definitions').deleteMany({});
        await db.collection('content_packages').deleteMany({});
        await db.collection('price_series_items').deleteMany({});
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
