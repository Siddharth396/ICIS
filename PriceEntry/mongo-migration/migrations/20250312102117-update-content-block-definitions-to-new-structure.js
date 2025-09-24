/* eslint-disable max-len */
const { v4: uuidv4 } = require('uuid');

module.exports = {
  async up(db, client) {
    console.log('Running migration 20250312102117-update-content-block-definitions-to-new-structure');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        // Find unique contentblockid as we have content block with same contentblockid and diff version
        const uniqueContentBlockIds = await db.collection('content_block_definitions').distinct(
          'content_block_id',
          {},
          { session },
        );

        /** for each unique contentblockid
            1.Check if contentblock has price_series_ids exists. We don't need to add price series grid
              if content block doesn't have price_series_ids.
            2.If price_series_ids exists then add new property price_series_grids with price_series_grid_id,
              price_series_ids and series_item_type_code
         Note : In case if we have multiple version of same contentblock it will keep price_series_grid_id same in each version of contentblock
        ** */
        await Promise.all(uniqueContentBlockIds.map(async (contentBlockId) => {
          await db.collection('content_block_definitions').updateMany(
            {
              $and: [
                { content_block_id: contentBlockId },
                { price_series_ids: { $ne: null } },
              ],
            },
            [{
              $set: {
                price_series_grids: [
                  {
                    price_series_grid_id: uuidv4(),
                    title: '$title',
                    price_series_ids: '$price_series_ids',
                    series_item_type_code: '$series_item_type_code',
                  },
                ],
              },
            }],
            { session },
          );
        }));
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
