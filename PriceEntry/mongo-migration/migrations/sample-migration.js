module.exports = {
  async up(db, client) {
    console.log('Running migration [ADD-MIGRATION-NAME-HERE]');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        // TODO write your migration here.
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
