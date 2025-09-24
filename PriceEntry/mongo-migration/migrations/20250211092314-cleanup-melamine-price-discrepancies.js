module.exports = {
  async up(db, client) {
    console.log('Running migration 20250211092314-cleanup-melamine-price-discrepancies');
    const session = client.startSession();
    try {
      await session.withTransaction(async () => {
        // Series items to delete
        const melamineAsiaSEChina20241105SeriesItemId = '3a581da4-4b60-4ee6-bb99-cb769f081dcd';
        const melamineAsiaSECFR20241105SeriesItemId = 'c9ec96b0-76cd-4b4a-b56d-ae3ee754e0b7';
        const melamineChinaFOB20241105SeriesItemId = 'eb033cb5-878f-4fbd-ab76-3316ab7f9a0e';
        const melamineEuropeMonthlyContract20240925SeriesItemId = '59260994-2dc3-4884-938b-d4e119b032ee';
        const melamineEuropeQuarterlyContract20240626SeriesItemId = '149cdad0-1bc3-4bc5-9258-56f44431e3d3';
        const melamineEuropeQuarterlyContract20240925SeriesItemId = '4869050b-f3eb-4913-9fd7-023f98ce9659';
        const melamineBulkFOBContract20240424SeriesItemId = '711f338a-ce47-4445-8e9d-516f61de26ea';
        const melamineBulkFOBContract20240626SeriesItemId = 'dcf7f213-d523-402c-a381-6633cc17e515';

        const seriesItemIdsToDelete = [
          melamineAsiaSEChina20241105SeriesItemId,
          melamineAsiaSECFR20241105SeriesItemId,
          melamineChinaFOB20241105SeriesItemId,
          melamineEuropeMonthlyContract20240925SeriesItemId,
          melamineEuropeQuarterlyContract20240626SeriesItemId,
          melamineEuropeQuarterlyContract20240925SeriesItemId,
          melamineBulkFOBContract20240424SeriesItemId,
          melamineBulkFOBContract20240626SeriesItemId,
        ];

        // Data packages to delete
        const melamineAsia20241105DataPackageId = 'cb:759c1c73-0e02-416e-b32d-6409073c72d9_v:3_d:2024-11-05';
        const melamineEuropeMonthly20240925DataPackageId = 'cb:b637b0a4-3410-4770-afcf-4acb6930cc1f_v:2_d:2024-09-25';
        const melamineEuropeQuarterly20240626DataPackageId = 'cb:eeb3844d-60c3-41fa-9bb4-8b55cc85d9ce_v:2_d:2024-06-26';
        const melamineEuropeQuarterly20240925DataPackageId = 'cb:eeb3844d-60c3-41fa-9bb4-8b55cc85d9ce_v:2_d:2024-09-25';
        const melamineBulk20240424DataPackageId = 'cb:82af6f3c-09a4-4552-993a-8b64db168c11_v:3_d:2024-04-24';
        const melamineBulk20240626DataPackageId = 'cb:82af6f3c-09a4-4552-993a-8b64db168c11_v:3_d:2024-06-26';

        const dataPackagesIdsToDelete = [
          melamineAsia20241105DataPackageId,
          melamineEuropeMonthly20240925DataPackageId,
          melamineEuropeQuarterly20240626DataPackageId,
          melamineEuropeQuarterly20240925DataPackageId,
          melamineBulk20240424DataPackageId,
          melamineBulk20240626DataPackageId,
        ];

        // Delete all price_series_items, content_packages and pricing_content_packages
        await db.collection('price_series_items').deleteMany({
          _id: { $in: seriesItemIdsToDelete },
        }, { session });

        await db.collection('content_packages').deleteMany({
          _id: { $in: dataPackagesIdsToDelete },
        }, { session });

        await db.collection('pricing_content_packages').deleteMany({
          _id: { $in: dataPackagesIdsToDelete },
        }, { session });
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  },
};
