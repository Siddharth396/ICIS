module.exports = {
  async up(db, client) {
    console.log('Running migration 20250502072506-update_dlhrecordsource_price_series_items');
    const session = client.startSession();
    try {
      const lastModified = {
        timestamp: new Date(),
        user: 'migration-of-dlhrecordsource',
      };
      const dbMapping = {
        'prcent-systest': 'canvas-prcent-systest',
        'prcent-integration': 'canvas-prcent-integration',
        'prcent-performance': 'canvas-prcent-performance',
        'prcent-prod': 'canvas-prcent-prod',
        'prcent-onboarding': 'canvas-prcent-onboarding',
        'prcent-staging': 'canvas-prcent-uat',
      };
      const dbNameKey = process.env.MONGO_DB_NAME.toLowerCase().trim();
      const dlhRecordSource = dbMapping[dbNameKey];
      if (dlhRecordSource) {
        await session.withTransaction(async () => {
          await db.collection('price_series_items').updateMany({
            $or: [{
              dlh_record_source: {
                $exists: false,
              },
            }, {
              dlh_record_source: null,
            }],
          }, [{
            $set: {
              dlh_record_source: dlhRecordSource,
              last_modified: lastModified,
            },
          }], {
            session,
          });
        });
      } else {
        console.log(`Invalid dlhRecordSource for DB name: ${dbNameKey}`);
      }
    } finally {
      await session.endSession();
    }
  },
  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
