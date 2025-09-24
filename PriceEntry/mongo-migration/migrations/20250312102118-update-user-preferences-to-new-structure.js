/* eslint-disable max-len */
module.exports = {
  async up(db, client) {
    console.log('Running migration 20250312102118-update-user-preferences-to-new-structure');
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
            1.Find contentblock details if price_series_grids is exist
            2.Extract price_series_grid_id from contentblock details
            3.Update all user_preferences for a given contentblock. We need to put the check of price_series_grids to make sure
              we don't do migration of user_preferences which already have new structure
        * */
        await Promise.all(uniqueContentBlockIds.map(async (contentBlockId) => {
          const contentBlock = await db.collection('content_block_definitions').findOne(
            {
              content_block_id: contentBlockId,
              price_series_grids: { $exists: true, $not: { $size: 0 }, $ne: null },
            },
            { session },
          );

          if (contentBlock) {
            const priceSeriesGridId = contentBlock.price_series_grids[0].price_series_grid_id;

            if (priceSeriesGridId) {
              await db.collection('user_preferences').updateMany(
                {
                  content_block_id: contentBlockId,
                  $or: [
                    { price_series_grids: { $exists: false } },
                    { price_series_grids: { $exists: true, $size: 0 } },
                  ],
                },
                [{
                  $set: {
                    price_series_grids: [
                      {
                        price_series_grid_id: priceSeriesGridId,
                        columns: '$columns',
                        price_series_ids: '$price_series',
                      },
                    ],
                  },
                }],
                { session },
              );
            }
          }
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
