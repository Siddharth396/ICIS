module.exports = {
  async up(db, client) {
    console.log('Running migration 20250604131632-data-package-metadata-index');
    
    // Check if the index already exists
    const indexInfo = await db.collection('data_package_metadata').indexInformation();
    const indexExists = Object.values(indexInfo).some(idx => 
      idx.length === 3 && 
      idx.some(field => field[0] === 'data_package_id_metadata.content_block_id') &&
      idx.some(field => field[0] === 'data_package_id_metadata.content_block_version') &&
      idx.some(field => field[0] === 'data_package_id_metadata.applies_from')
    );
    
    if (indexExists) {
      console.log('Index on data_package_id_metadata fields already exists. Skipping creation.');
      return;
    }

    await db.collection('data_package_metadata').createIndex(
      {
        'data_package_id_metadata.content_block_id': 1,
        'data_package_id_metadata.content_block_version': 1,
        'data_package_id_metadata.applies_from': 1,
      },
      {
        unique: true,
        name: 'data_package_id_metadata_unique_idx',
      }
    );
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
