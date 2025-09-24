module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703071110-rename-applies-from-to-assessed-datetime-for-content-packages');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('content_packages').updateMany(
          { applies_from: { $exists: true } },
          { $rename: { applies_from: 'assessed_datetime' } },
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
