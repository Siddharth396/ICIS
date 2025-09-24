module.exports = {
  async up(db, client) {
    console.log('Running migration 20250514064201-delete-duplicate-datapackagemetadata-for-14th-may');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('data_package_metadata').deleteOne({
          _id: '7d5e37ba-4912-4f92-a905-7c2e29dd733f',
        }, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
