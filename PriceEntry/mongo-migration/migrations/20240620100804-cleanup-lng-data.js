module.exports = {
  async up(db, client) {
    console.log('Running migration 20240620100804-cleanup-lng-data');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const priceItemsCondition = { series_item_type_code: 'single-value-ref' };
        await db.collection('price_series_items').deleteMany(priceItemsCondition);

        const contentPackagesCondition = { 'content_block.series_item_type_code': 'single-value-ref' };
        await db.collection('content_packages').deleteMany(contentPackagesCondition);
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
