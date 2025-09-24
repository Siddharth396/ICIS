/* eslint-disable max-len */
/**
 * This migration script creates a MongoDB view named 'vw_data_package_with_series_items'.
 * The view joins 'content_packages' with 'price_series_items' and conditionally adds or updates
 * the fields 'price_delta', 'price_low_delta', and 'price_high_delta' based on the existence
 * and values of their corresponding 'adjusted' fields.
 * If the view already exists, it is dropped before creating the new view.
 *
 * High-Level Explanation:
 * The $addFields section in the MongoDB aggregation pipeline is used to conditionally add or update
 * fields in the documents of the 'series_items' array. It ensures that the fields 'price_delta',
 * 'price_low_delta', and 'price_high_delta' are set based on the existence and values of their
 * corresponding 'adjusted' fields. If the 'adjusted' fields are present and not null, their values
 * are used; otherwise, the original values are retained. If the original fields do not exist, they
 * are not added to the document.
 *
 * Detailed Explanation:
 * 1. Mapping Over 'series_items':
 *    - The $map operator iterates over each item in the 'series_items' array.
 *    - For each item, it creates a new object by merging the original item ('$$item') with the new fields.
 *
 * 2. Conditional Field Updates:
 *    - 'price_delta':
 *      - Checks if 'price_delta' exists in the item ('$ne: [{ $type: '$$item.price_delta' }, 'missing']').
 *      - If it exists, it further checks if 'adjusted_price_delta' is not null ('$ne: ['$$item.adjusted_price_delta', null]').
 *      - If 'adjusted_price_delta' is not null, it sets 'price_delta' to 'adjusted_price_delta'.
 *      - Otherwise, it retains the original 'price_delta'.
 *      - If 'price_delta' does not exist, it is not added to the document (using '$$REMOVE').
 *
 *    - 'price_low_delta':
 *      - Similar logic as 'price_delta' is applied to 'price_low_delta' and 'adjusted_price_low_delta'.
 *
 *    - 'price_high_delta':
 *      - Similar logic as 'price_delta' is applied to 'price_high_delta' and 'adjusted_price_high_delta'.
 *
 *    - 'price_mid_delta':
 *      - Similar logic as 'price_delta' is applied to 'price_mid_delta' and 'adjusted_price_mid_delta'.
 */

module.exports = {
  async up(db, client) {
    console.log('Running migration 20250227105602-update-data-package-and-series-items-view-adjusted-mid-price');
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
                localField: 'price_series_item_ids',
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
