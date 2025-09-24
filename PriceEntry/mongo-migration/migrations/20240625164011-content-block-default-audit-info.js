module.exports = {
  async up(db, client) {
    console.log('Running migration 20240625164011-content-block-default-audit-info');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('content_block_definitions').updateMany(
          {
            last_modified: { $exists: false },
          },
          {
            $set: {
              last_modified: {
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
