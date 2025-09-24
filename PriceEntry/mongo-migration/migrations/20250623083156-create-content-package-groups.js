const crypto = require('crypto'); 
const { v4: uuidv4 } = require('uuid');

module.exports = {
  async up(db, client) {
    console.log('Running migration 20250623083156-create-content-package-groups');
    const session = client.startSession();

    try {
      await session.withTransaction(async () => {
        // Step 1: Aggregate unique combinations of price_series_ids and associated content_block_ids/versions
        const contentBlockDefinitions = db.collection('content_block_definitions');
        const contentPackageGroups = db.collection('content_package_groups');

        const aggregationResult = await contentBlockDefinitions.aggregate([
          // Unwind the price_series_grids array to process each grid individually
          { $unwind: '$price_series_grids' },
          // Unwind the price_series_ids array within each grid
          { $unwind: '$price_series_grids.price_series_ids' },
          // Group by content_block_id and version, and collect unique price_series_ids
          {
            $group: {
              _id: {
                content_block_id: '$content_block_id',
                version: '$version'
              },
              priceSeriesIds: { $addToSet: '$price_series_grids.price_series_ids' }
            }
          },
          {
            $project: {
              _id: 1,
              priceSeriesIds: { $setUnion: ['$priceSeriesIds', []] } // Deduplicate and sort
            }
          },
          // Regroup by unique combinations of price_series_ids
          {
            $group: {
              _id: '$priceSeriesIds', // Group by unique price_series_ids
              contentBlockDefinitions: {
                $push: {
                  content_block_id: '$_id.content_block_id',
                  version: '$_id.version'
                }
              }
            }
          },
          // Rename _id to priceSeriesIds for clarity
          {
            $project: {
              _id: 0,
              priceSeriesIds: '$_id',
              contentBlockDefinitions: 1
            }
          },
          // Optionally, sort the results for better readability
          { $sort: { priceSeriesIds: 1 } }
        ]).toArray();

        // Step 2: Insert into content_package_groups
        const priceSeriesGroups = aggregationResult || [];

        const contentPackageGroupDocs = priceSeriesGroups.map(group => {
          // Sort price_series_ids alphabetically
          const sortedPriceSeriesIds = group.priceSeriesIds.sort();

          // Concatenate sorted price_series_ids into a single string
          const concatenatedIds = sortedPriceSeriesIds.join(',');

          // Generate a hash of the concatenated string
          const hash = crypto.createHash('sha256').update(concatenatedIds).digest('hex');
    
          // sort content block definitions by content_block_id first and then by version
          const sortedContentBlockDefinitions = group.contentBlockDefinitions.sort((a, b) => {
            if (a.content_block_id === b.content_block_id) {
              return a.version - b.version;
            }
            return a.content_block_id.localeCompare(b.content_block_id);
          });
          
          return {
            _id: uuidv4(),
            sequence_id: hash, // Use the hash as the sequence_id
            price_series_ids: group.priceSeriesIds, // The grouped price_series_ids
            content_block_definitions: sortedContentBlockDefinitions // Associated content_block_definitions
          };
        });

       await contentPackageGroups.insertMany(contentPackageGroupDocs, { session });
       console.log('Inserted content_package_groups:', contentPackageGroupDocs);

      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};