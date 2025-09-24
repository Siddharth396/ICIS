module.exports = {
  async up(db, client) {
    console.log('Running migration 20250530083441-delete-duplicate-datapackagemetadata-for-30th-may');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        await db.collection('data_package_metadata').deleteMany({
           _id: {
             $in: [
                '816d812b-b056-49f8-a80b-02880d4ee120',
                '8f36b7ce-aa49-4c7c-b56a-a3483bfcadd4',
                'c8ff1355-2138-4b7c-a510-0339b8fc1d0f',
                '12f17562-a215-4cc5-aa95-857d378d212f',
                '1223cb4d-5357-4dc1-aa99-3001a459734c',
                'b2c598d4-4dc8-4e02-9548-02d94ddf6677',
                '3809bf84-1d72-41f9-9939-9ad3805cfef9',
                '7ea40f06-8913-4242-ae8a-0b4afae22800',
                '7b1c249c-3cb3-4c6d-8150-d210c29e8fe2'
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
