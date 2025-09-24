module.exports = {
  async up(db, client) {
    console.log('Running migration 20250604131630-data-package-metadata-cleanup-duplicates');

    const session = client.startSession();
    try {
      await session.withTransaction(async () => {

        // Check for duplicates before creating the unique index (next script)
        const duplicates = await db.collection('data_package_metadata').aggregate([
          {
            $group: {
              _id: {
                content_block_id: '$data_package_id_metadata.content_block_id',
                content_block_version: '$data_package_id_metadata.content_block_version',
                applies_from: '$data_package_id_metadata.applies_from',
              },
              count: { $sum: 1 },
              docs: { $push: '$_id' }
            }
          },
          { $match: { count: { $gt: 1 } } }
        ], { session }).toArray();

        if (duplicates.length > 0) {
          console.log(`Found ${duplicates.length} sets of duplicates. Deduplicating...`);

          // Process each group of duplicates
          for (const duplicate of duplicates) {
            // Keep the first document and delete the rest
            const docsToKeep = duplicate.docs[0]; // Keep the first one
            const docsToRemove = duplicate.docs.slice(1); // Remove the rest

            console.log(`Keeping document ${docsToKeep}, removing ${docsToRemove.length} duplicates`);

            // Delete the duplicate documents
            await db.collection('data_package_metadata').deleteMany({
              _id: { $in: docsToRemove }
            }, { session });
          }

          console.log('Deduplication complete');
        } else {
          console.log('No duplicates found');
        }
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
