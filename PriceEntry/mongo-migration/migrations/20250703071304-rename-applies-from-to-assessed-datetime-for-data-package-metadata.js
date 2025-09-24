module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703071304-rename-applies-from-to-assessed-datetime-for-data-package-metadata');
     const session = client.startSession();
    try {
       await session.withTransaction(async () => {
        await db.collection('data_package_metadata').updateMany(
          { 'data_package_id_metadata.applies_from': { $exists: true } },
          { $rename: { 'data_package_id_metadata.applies_from': 'data_package_id_metadata.assessed_datetime' } },
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
