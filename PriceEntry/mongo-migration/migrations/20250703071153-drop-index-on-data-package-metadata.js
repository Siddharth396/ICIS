module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703071153-drop-index-on-data-package-metadata');
     const session = client.startSession();
    try {
       await session.withTransaction(async () => {
        await db.collection('data_package_metadata').dropIndex(
          {
            'data_package_id_metadata.content_block_id': 1,
            'data_package_id_metadata.content_block_version': 1,
            'data_package_id_metadata.applies_from': 1,
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
