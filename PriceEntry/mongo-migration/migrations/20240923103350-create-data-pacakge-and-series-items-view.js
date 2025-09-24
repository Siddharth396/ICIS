module.exports = {
  async up(db, client) {
    console.log('Running migration 20240923103350-create-data-pacakge-and-series-items-view');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        // the view name is how it will be when we rename content_pacakges to data_packages
        await db.createCollection('vw_data_package_with_series_items', {
          viewOn: 'content_packages',
          pipeline: [
            {
              $lookup: {
                from: 'price_series_items',
                localField: 'price_series_item_ids',
                foreignField: '_id',
                as: 'series_items',
              },
            },
          ],
        });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
