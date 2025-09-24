/* eslint-disable max-len */
module.exports = {
  async up(db, client) {
    console.log('Running migration 20250402134430-lng-advance-workflow-data-package-migration');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        /** Old structure
           _id : cb:2fbeaeb4-0669-413c-892b-6a03f080e6ec_v:1_d:2024-09-17
           ...
           "content_block": {
                  "_id": "2fbeaeb4-0669-413c-892b-6a03f080e6ec",
                  "version": 1,
                  "series_item_type_code": "pi-single-with-ref"
               },
            "price_series_item_ids": [
                "f38755a1-48ec-412b-8fa3-b71d5060e253",
                "0447de8b-d519-4c82-a9b0-1141e55e27d2",
                "13f92d3d-fa5f-46d8-b6e4-9c574a0f2ed8",
                "c2f82bff-8c63-407a-845c-252a40e6a332",
                "60c1e058-361f-4609-8ce8-629d4fa160d1"
              ],

           New structure
           _id : cb:2fbeaeb4-0669-413c-892b-6a03f080e6ec_v:1_d:2024-09-17
           ...
          "price_series_item_groups": [
              {
                "price_series_item_ids": [
                  "f38755a1-48ec-412b-8fa3-b71d5060e253",
                  "0447de8b-d519-4c82-a9b0-1141e55e27d2",
                  "13f92d3d-fa5f-46d8-b6e4-9c574a0f2ed8",
                  "c2f82bff-8c63-407a-845c-252a40e6a332",
                  "60c1e058-361f-4609-8ce8-629d4fa160d1"
                ],
              "series_item_type_code": "pi-single-with-ref"
            },
         ]
         */

        await db.collection('content_packages').updateMany(
          {},
          [{
            $set: {
              price_series_item_groups: [
                {
                  price_series_item_ids: '$price_series_item_ids',
                  series_item_type_code: '$content_block.series_item_type_code',
                },
              ],
            },
          }],
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
