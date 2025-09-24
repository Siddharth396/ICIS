module.exports = {
  async up(db, client) {
    console.log('Running migration 20241016102543-update-reference-price-fields');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        db.collection('price_series_items').updateMany(
          { reference_price: { $ne: null } },
          {
            $rename: {
              'reference_price.series': 'reference_price.series_name',
            },
          },
          { session },
        );

        const aggregationResult = await db.collection('price_series_items').aggregate([
          {
            $match: {
              $and: [
                { reference_price: { $ne: null } },
                { 'reference_price.market': { $in: ['LNG DES Argentina', 'LNG FOB Reload NWE', 'LNG DES India', 'LNG EAX Index'] } },
              ],
            },
          },
          {
            $lookup: {
              from: 'price_series',
              let: { seriesname: '$reference_price.series_name' },
              pipeline: [
                {
                  $match: {
                    $expr: {
                      $or: [
                        { $eq: ['$series_short_name.en', '$$seriesname'] },
                        { $eq: ['$series_name.en', '$$seriesname'] },
                      ],
                    },
                  },
                },
              ],
              as: 'price_series_details',
            },
          },
          {
            $unwind: {
              path: '$price_series_details',
            },
          },
          {
            $addFields: {
              reference_series_id: '$price_series_details._id',
            },
          },
        ]).toArray();

        await Promise.all(
          aggregationResult.map((doc) => db.collection('price_series_items').updateOne(
            { _id: doc._id },
            {
              $set: {
                'reference_price.series_id': doc.reference_series_id,
              },
            },
            { session },
          )),
        );
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
