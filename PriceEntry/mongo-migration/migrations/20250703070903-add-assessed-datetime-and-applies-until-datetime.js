module.exports = {
  async up(db, client) {
    console.log('Running migration 20250703070903-add-assessed-datetime-and-applies-until-datetime');
    const session = client.startSession();
        const BATCH_SIZE = 10000;
        const collection = db.collection('price_series_items');

        try {
          const priceSeriesItems = await collection
            .find({},
              {
                projection: {
                  _id: 1,
                  applies_from_datetime: 1
                }
              },
              { session }
            ).toArray();

          let bulkOps = [];
          let updatedCount = 0;

          for (let i = 0; i < priceSeriesItems.length; i++) {
            const priceSeriesItem = priceSeriesItems[i];

            bulkOps.push({
              updateOne: {
                filter: { _id: priceSeriesItem._id },
                update: {
                  $set: {
                    assessed_datetime: priceSeriesItem.applies_from_datetime,
                    applies_until_datetime: null
                  }
                }
              }
            });

            // Process in batches
            if (bulkOps.length === BATCH_SIZE || i === priceSeriesItems.length - 1) {
              await session.withTransaction(async () => {
                const result = await collection.bulkWrite(bulkOps, { session });
                updatedCount += result.modifiedCount;
                console.log(`Batch updated: ${result.modifiedCount} docs`);
              });

              bulkOps = [];
            }
          }

          console.log(`Migration complete. Total updated: ${updatedCount}`);
        } catch (err) {
          console.error('Migration failed:', err.message);
          throw err;
        } finally {
          await session.endSession();
        }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
