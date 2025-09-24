module.exports = {
  async up(db, client) {
    console.log('Running migration 20250519061611-remove-unnecessary-properties');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        
      await db.collection('content_block_definitions').updateMany(
        {},
        {
          $unset: { 
            price_series_ids: '',
            series_item_type_code: ''
          },
        },
        { session },
      );

       await db.collection('content_packages').updateMany(
        {},
        {
          $unset: { 
            price_series_item_ids: '',
            'content_block.series_item_type_code': ''
          },
        },
        { session },
      );

      await db.collection('user_preferences').updateMany(
        {},
        {
          $unset: { 
            columns: '',
            price_series: ''
          },
        },
        { session },
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
