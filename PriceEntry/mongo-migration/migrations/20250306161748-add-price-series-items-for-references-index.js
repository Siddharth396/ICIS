module.exports = {
  async up(db, client) {
    console.log('Running migration 20250306161748-add-price-series-items-for-references-index');
    try {
      await db.collection('price_series_items')
        .createIndex(
          {
            applies_from_datetime: 1,
            'reference_price.series_id': 1,
          },
        );
    } finally {
      console.log('migration 20250306161748-add-price-series-items-for-references-index complete');
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
