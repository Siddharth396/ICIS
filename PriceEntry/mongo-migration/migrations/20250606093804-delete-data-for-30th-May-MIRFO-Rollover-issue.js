module.exports = {
  async up(db, client) {
    console.log('Running migration 20250606093804-delete-data-for-30th-May-MIRFO-Rollover-issue');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const appliesFromDateTime = '2025-05-30T00:00:00.000+00:00';
        const seriesId = 'ed81dd0e-f397-4029-8176-8f1f839ac4e8';

        // Find all content block that belong to the series
        const contentBlocksAggregationResult = await db.collection('content_block_definitions').find({
          'price_series_grids.price_series_ids': { $in: [seriesId] }
        }, { session }).toArray();

        const contentBlockIds = contentBlocksAggregationResult.map((doc) => doc.content_block_id);
        
        // Find all series items that belong to the series
        const seriesItemsAggregationResult = await db.collection('price_series_items').find({
          series_id: seriesId,
          applies_from_datetime: { $eq: new Date(appliesFromDateTime) }
        }, { session }).toArray();

        const seriesItemIds = seriesItemsAggregationResult.map((doc) => doc._id);
        
        // Find datapackage metadata id
        const dataPackageMetadataAggregationResult = await db.collection('data_package_metadata').find({
          'data_package_id_metadata.content_block_id': { $in: contentBlockIds },
          'data_package_id_metadata.applies_from': { $eq: new Date(appliesFromDateTime) }
        }, { session }).toArray();

        const dataPackageMetadataId = dataPackageMetadataAggregationResult.map((doc) => doc._id);

        // Delete all price_series_items, content_packages, data_package_metadata, pricing_content_packages and commentaries
        await db.collection('price_series_items').deleteOne({
          _id: { $in: seriesItemIds }
        }, { session });

        await db.collection('data_package_metadata').deleteOne({
          _id: { $in: dataPackageMetadataId }
        }, { session });

        await db.collection('content_packages').deleteOne({
          _id: { $in: dataPackageMetadataId }
        }, { session });

        await db.collection('pricing_content_packages').deleteOne({
          _id: { $in: dataPackageMetadataId }
        }, { session });

        await db.collection('commentaries').deleteOne({
          content_block_id: { $in: contentBlockIds },
          applies_from_datetime: { $eq: new Date(appliesFromDateTime) }
        }, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
