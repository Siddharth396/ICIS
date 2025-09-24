
const { ensureCollectionExists } = require('../helpers/collectionFunctions');

module.exports = {
  async up(db, client) {
    console.log('Running migration 20250620045416-content-package-groups-index');

    const collectionName = 'content_package_groups';

    // check if content_package_groups collection exists. If not, create it.
    await ensureCollectionExists(db, collectionName);

    // Check if the index already exists
    const indexInfo = await db.collection(collectionName).indexInformation();
    const indexExists = Object.values(indexInfo).some(idx => 
      idx.length === 1 && 
      idx.some(field => field[0] === 'sequence_id')
    );
    
    if (indexExists) {
      console.log('Index on content_package_groups fields already exists. Skipping creation.');
      return;
    }

    await db.collection(collectionName).createIndex(
      {
        sequence_id: 1
      },
      {
        unique: true,
        name: 'unique_sequence_id_idx',
      }
    );
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
