module.exports = {
  async up(db, client) {
    console.log('Running migration 20240625162305-content-package-default-audit-info');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('content_packages').updateMany(
          {
            $or: [
              { last_modified: { $exists: false } },
              { created: { $exists: false } },
            ],
          },
          {
            $set: {
              last_modified: {
                timestamp: new Date(),
                user: 'migration',
              },
              created: {
                timestamp: new Date(),
                user: 'migration',
              },
            },
          },
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
