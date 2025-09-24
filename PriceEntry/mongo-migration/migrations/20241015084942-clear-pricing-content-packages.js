module.exports = {
  async up(db, client) {
    console.log('Running migration 20241015084942-clear-pricing-content-packages');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('pricing_content_packages').deleteMany({}, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
