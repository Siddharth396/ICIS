module.exports = {
  async up(db, client) {
    console.log('Running migration 20240622070114-add-reference-markets');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const referenceMarkets = [
          {
            _id: 'TTF',
            name: 'TTF',
            type: 'gas',
          },
          {
            _id: 'NBP',
            name: 'NBP',
            type: 'gas',
          },
          {
            _id: 'THE',
            name: 'THE',
            type: 'gas',
          },
          {
            _id: 'PSV',
            name: 'PSV',
            type: 'gas',
          },
          {
            _id: 'PEG',
            name: 'PEG',
            type: 'gas',
          },
          {
            _id: 'PVB',
            name: 'PVB',
            type: 'gas',
          },
          {
            _id: 'ZTP',
            name: 'ZTP',
            type: 'gas',
          },
          {
            _id: 'LNG_DES_Argentina',
            name: 'LNG DES Argentina',
            type: 'lng',
            location: 'Argentina',
          },
          {
            _id: 'LNG_FOB_Reload_NWE',
            name: 'LNG FOB Reload NWE',
            type: 'lng',
            location: 'Zeebrugge',
          },
        ];
        await db.collection('reference_markets').insertMany(referenceMarkets);
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
