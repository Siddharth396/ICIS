module.exports = {
  async up(db, client) {
    console.log('Running migration 20250421060053-add-new-reload-nwe-fob-reference-market');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('reference_markets').updateMany(
          { _id: 'LNG_FOB_Reload_NWE' },
          {
            $set: {
              price_series_ids: [
                '3df642f2-7426-43dc-b347-86ce770043ca',
                '00d44d24-424e-475a-ae32-77ce43af4893',
              ],
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
