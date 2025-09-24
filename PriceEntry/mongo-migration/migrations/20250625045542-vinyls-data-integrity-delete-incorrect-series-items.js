const { deletePriceSeriesItems } = require('../helpers/deletePriceSeriesItemFunction');

module.exports = {
  async up(db, client) {
    console.log('Running migration 20250625045542-vinyls-data-integrity-delete-incorrect-series-items');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        const priceSeriesItemIds = [
          '68763efc-1679-4598-96fb-e814e44fd2e6',
          'fd0bf08d-3aaa-4117-bc0d-2fa5e3b410fa',
          '02d57d48-3c2d-425d-8938-282d444b5a2b',
          '026755f3-ea76-49f0-bf78-84f8ebb47880',
          'fc826f83-cab9-428d-9cd0-03ceae425cd2',
          '4b1b4833-37a3-4b8f-991f-f59220dea3a1', 
          '991197b3-10db-4bcf-be55-bea4081bc933', 
          '4aa2aff6-31b1-4281-bd27-fa12d471b516',
          'b23d92a0-0d46-45e7-a333-c49b7d9c592a',
          '6aaa8602-74b0-4b2c-993b-e1f4ea7c84ae',
          '9690a948-eb7e-4e8b-9c0e-b66f4d42503b',
        ];

        await deletePriceSeriesItems(db, priceSeriesItemIds, session);

      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
