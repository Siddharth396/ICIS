module.exports = {
  async up(db, client) {
    console.log('Running migration 20250127100506-create-data-package-metadata-collection');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const data = await db.collection('content_packages').aggregate([
          {
            $project:
              {
                _id: '$_id',
                data_package_id_metadata: {
                  content_block_id: '$content_block._id',
                  content_block_version: '$content_block.version',
                  applies_from: '$applies_from',
                },
              },
          },
        ], { session }).toArray();

        // Exclude content packages that already have the metadata with the same id.
        // This is to ensure that the migration can be run after BFF has been deployed.
        const bulkOps = data.map((doc) => ({
          updateOne: {
            filter: { _id: doc._id },
            update: { $setOnInsert: doc },
            upsert: true,
          },
        }));

        await db.collection('data_package_metadata').bulkWrite(bulkOps, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
