module.exports = {
  async up(db, client) {
    console.log('Running migration 20241016144441-create-price-display-content-block-definitions-collection');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.createCollection('price_display.content_block_definitions');
        console.log('Collection price_display.content_block_definitions created');

        await db.collection('price_display.content_block_definitions')
          .createIndex(
            {
              content_block_id: 1,
              version: 1,
            },
            {
              unique: true,
            },
          );
        console.log('Unique index on content_block_id and version created');
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
