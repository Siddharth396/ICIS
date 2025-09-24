module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703071355-recreate-index-on-data-package-metadata');
     const session = client.startSession();
    try {
       await session.withTransaction(async () => {
        await db.collection('data_package_metadata').createIndex(
          {
            'data_package_id_metadata.content_block_id': 1,
            'data_package_id_metadata.content_block_version': 1,
            'data_package_id_metadata.assessed_datetime': 1,
          },
          {
            unique: true,
            partialFilterExpression: {
              'data_package_id_metadata.assessed_datetime': { $exists: true, $type: 'date'  }
            },
            name: 'data_package_id_metadata_unique_idx',
          }
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
