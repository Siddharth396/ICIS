const { deletePriceSeriesItems } = require('../helpers/deletePriceSeriesItemFunction');

module.exports = {
  async up(db, client) {
    console.log('Running migration 20250708064340-delete-data-caustic-soda-us-7th-July');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const priceSeriesItemIds = [
          'cac732d3-b51e-415a-82c7-470ae06d72dc',
        ];

        await deletePriceSeriesItems(db, priceSeriesItemIds, session);
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
