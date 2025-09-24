module.exports = {
  async up(db, client) {
    console.log('Running migration 20241129113522-cleanup-melamine-quarterly-contract-price');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const seriesItemId = '2850a117-b33e-4193-a49a-e0c3462d9190';
        const contentPackageId = 'cb:eeb3844d-60c3-41fa-9bb4-8b55cc85d9ce_v:2_d:2024-10-30';

        // Delete price_series_item, content_package and pricing_content_packages
        await db.collection('price_series_items').deleteOne({
          _id: seriesItemId,
        }, { session });

        await db.collection('content_packages').deleteOne({
          _id: contentPackageId,
        }, { session });

        await db.collection('pricing_content_packages').deleteOne({
          _id: contentPackageId,
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
