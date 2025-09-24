const { dateToString } = require('../helpers/dateFunctions');

module.exports = {
  async up(db, client) {
    console.log('Running migration 20240604082211-content-package-price-series-items-ids');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const docs = await db.collection('content_packages').find({}).toArray();

        console.log(`Found ${docs.length} content packages`);

        await Promise.all(docs.map(async (doc) => {
          const formattedDate = dateToString(doc.applies_from);
          const priceSeriesItemIds = doc.price_series_ids ? doc.price_series_ids.map((item) => `${item}_${formattedDate}`) : [];

          if (priceSeriesItemIds.length > 0) {
            await db.collection('content_packages').updateOne(
              { _id: doc._id },
              {
                $set: { price_series_item_ids: priceSeriesItemIds },
                $unset: { price_series_ids: 1 },
              },
            );
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
