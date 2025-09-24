module.exports = {
  async up(db, client) {
    console.log('Running migration 20250603042517-delete_price_series_items_02May2025');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('price_series_items').deleteMany({
          _id: {
            $in: [
               '126da926-ff9b-4309-9ef3-91a91a8d564d',
               'd4b33807-b08b-4b42-8f10-0882476f16da',
               '225c86c1-1e02-439e-9014-cefd147afabf',
               'f378850c-6b43-4c04-b093-7f4d8793a31f',
               'e24d1a00-d70d-4dfa-aa17-072454355890',
               '889444ca-8775-437d-8c79-a11292a447d2',
               '2551cd07-3a0f-45ad-9e9f-6f3e05c8e7f6',
               '995b7221-5bda-4980-9ee5-56120f70a27b',
               'a2effd14-706b-47ec-8d3c-76756f1d93c3',
               '42f4c2a9-a3b4-4c37-a783-4616d3a6aa36',
               'e2d1c393-201a-47d7-a4b5-e9b58bcc520b',
               '2f8fed8f-c634-4f73-bba4-cde9ebe1588d'
             ] 
           },
       }, 
      { session });

      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
