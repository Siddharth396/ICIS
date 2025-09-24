module.exports = {
  async up(db, client) {
    console.log('Running migration 20241015082222-update-documents-statuses');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const lastModified = {
          timestamp: new Date(),
          user: 'migration-of-statuses',
        };

        await db.collection('price_series_items').updateMany(
          { status: { $in: ['IN_DRAFT', 'PUBLISH_IN_PROGRESS'] } },
          {
            $set: {
              status: 'DRAFT',
              last_modified: lastModified,
            },
          },
          { session },
        );

        await db.collection('content_packages').updateMany(
          { status: { $in: ['None', 'Created', 'WorkflowInitialized', 'WorkflowTransitionFailed', 'WorkflowWaitingToPublish', 'CorrectionInitiated'] } },
          {
            $set: {
              status: 'DRAFT',
              last_modified: lastModified,
            },
          },
          { session },
        );

        await db.collection('content_packages').updateMany(
          { status: 'WorkflowPublished' },
          {
            $set: {
              status: 'PUBLISHED',
              last_modified: lastModified,
            },
          },
          { session },
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
