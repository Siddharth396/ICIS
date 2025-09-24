module.exports = {
  async up(db, client) {
    console.log('Running migration 20240912110600-cleanup-mongodb-data');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('price_series_items').deleteMany({}, { session });
        await db.collection('content_packages').deleteMany({}, { session });
        await db.collection('audit_logs').deleteMany({}, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
