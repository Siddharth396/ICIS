module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703071448-rename-applies-from-to-assessed-datetime-for-commentaries');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('commentaries').updateMany(
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
