module.exports = {
  async up(db, client) {
    console.log('Running migration 20240708035816-add-price-delta-type');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const priceDeltaType = [
          {
            guid: 'fe446f10-4901-4661-a281-44d2f9ba26e4',
            code: 'REGULAR',
            name: {
              en: 'regular',
            },
            description: {
              en: 'a delta calculated as the routine difference between adjacent items, showing the most recent trend of the price',
            },
            expired_as_of: null,
            is_deleted: false,
            created_on: new Date(),
            last_updated_on: new Date(),
          },
          {
            guid: '65f70709-3cfc-4e54-8913-759a2ec76d5b',
            code: 'NONMKTADJ',
            name: {
              en: 'non market adjustment',
            },
            description: {
              en: 'contextualising possible jumps in the price niveau owed to a re-baselining of underlying price',
            },
            expired_as_of: null,
            is_deleted: false,
            created_on: new Date(),
            last_updated_on: new Date(),
          },
        ];

        await db.collection('price_delta_types').insertMany(priceDeltaType);
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
