module.exports = {
  async up(db, client) {
    console.log('Running migration 20250402113448-update-data-package-with-series-items-view');
    const session = client.startSession();
    const viewName = 'vw_data_package_with_series_items';
    try {
      const collections = await db.listCollections({ name: viewName }).toArray();
      if (collections.length > 0) {
        await db.collection(viewName).drop();
      }
      await session.withTransaction(async () => {
        await db.createCollection(viewName, {
          viewOn: 'content_packages',
          pipeline: [
            {
              $lookup: {
                from: 'price_series_items',
                localField: 'price_series_item_groups.price_series_item_ids',
                foreignField: '_id',
                as: 'series_items',
              },
            },
            {
              $addFields: {
                series_items: {
                  $map: {
                    input: '$series_items',
                    as: 'item',
                    in: {
                      $mergeObjects: [
                        '$$item',
                        {
                          price_delta: {
                            $cond: {
                              if: { $ne: [{ $type: '$$item.price_delta' }, 'missing'] },
                              then: {
                                $cond: {
                                  if: { $ne: ['$$item.adjusted_price_delta', null] },
                                  then: '$$item.adjusted_price_delta',
                                  else: '$$item.price_delta',
                                },
                              },
                              else: '$$REMOVE',
                            },
                          },
                          price_low_delta: {
                            $cond: {
                              if: { $ne: [{ $type: '$$item.price_low_delta' }, 'missing'] },
                              then: {
                                $cond: {
                                  if: { $ne: ['$$item.adjusted_price_low_delta', null] },
                                  then: '$$item.adjusted_price_low_delta',
                                  else: '$$item.price_low_delta',
                                },
                              },
                              else: '$$REMOVE',
                            },
                          },
                          price_high_delta: {
                            $cond: {
                              if: { $ne: [{ $type: '$$item.price_high_delta' }, 'missing'] },
                              then: {
                                $cond: {
                                  if: { $ne: ['$$item.adjusted_price_high_delta', null] },
                                  then: '$$item.adjusted_price_high_delta',
                                  else: '$$item.price_high_delta',
                                },
                              },
                              else: '$$REMOVE',
                            },
                          },
                          price_mid_delta: {
                            $cond: {
                              if: { $ne: [{ $type: '$$item.price_mid_delta' }, 'missing'] },
                              then: {
                                $cond: {
                                  if: { $ne: ['$$item.adjusted_price_mid_delta', null] },
                                  then: '$$item.adjusted_price_mid_delta',
                                  else: '$$item.price_mid_delta',
                                },
                              },
                              else: '$$REMOVE',
                            },
                          },
                        },
                      ],
                    },
                  },
                },
              },
            },
          ],
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
