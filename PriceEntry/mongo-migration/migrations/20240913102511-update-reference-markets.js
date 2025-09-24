module.exports = {
  async up(db, client) {
    console.log('Running migration 20240913102511-update-reference-markets');
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
            price_series_ids: [
              '9981e4e0-3ad6-4e79-ba71-2a6ef1e23498',
              '141fb72a-e3c8-41d2-b794-8673233a43de',
            ],
          },
          {
            _id: 'LNG_FOB_Reload_NWE',
            name: 'LNG FOB Reload NWE',
            type: 'lng',
            price_series_ids: [
              '3df642f2-7426-43dc-b347-86ce770043ca',
            ],

          },
          {
            _id: 'LNG_DES_India',
            name: 'LNG DES India',
            type: 'lng',
            price_series_ids: [
              'f166184f-410b-4810-af22-5c4249531c82',
              '8cef4d5f-8ac5-438a-8fd4-f7df1770e675',
              'd20db60a-d39b-4884-8ac5-990e7631841e',
              '7ecf8404-0213-4a4e-98d5-d2ce4bf8190f',
              '6f37247a-ef9e-4ed4-9b55-d5de96316f1f',
              '9b10c254-a314-40b2-b396-b910d6c61c07',
              'ec83b252-e315-404a-a563-4deb690d595c',
            ],
          },
          {
            _id: 'LNG_EAX_Index',
            name: 'LNG EAX Index',
            type: 'lng',
            price_series_ids: [
              '9b22c4e3-b755-49ea-b8d0-7ca32a5d594c',
              '4ba9b510-89d0-4b04-8f6a-e8ec45b7576d',
              '0984da0d-6e1f-4ee0-966f-311b7c369947',
              'e068c499-984d-433b-bdbb-753f7a5e682c',
              '93fc51c3-cf09-4a0f-8192-e95d763c5c8d',
              'd05cfdae-0b35-4c67-a5d6-64c43759356f',
              'e656b2c2-12c0-4993-8e1b-7c3863fbafe1',
            ],
          },
        ];

        await db.collection('reference_markets').deleteMany({}, { session });
        await db.collection('reference_markets').insertMany(referenceMarkets, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
