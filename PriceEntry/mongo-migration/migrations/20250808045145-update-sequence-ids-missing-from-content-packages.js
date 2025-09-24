module.exports = {
  async up(db, client) {
    console.log('Running migration 20250808045145-update-sequence-ids-missing-from-content-packages');
    const session = client.startSession();

    try {
      await session.withTransaction(async () => {
        // Step 1: Get all content_package_groups with sequence_id
        const contentPackages = db.collection('content_packages');
        const pricingContentPackages = db.collection('pricing_content_packages');
        const contentPackageGroups = db.collection('content_package_groups');

        const allContentPackageGroups = await contentPackageGroups.find({}, { 
          projection: { sequence_id: 1, content_block_definitions: 1 }, 
          session 
        }).toArray();

        console.log(`Found ${allContentPackageGroups.length} content package groups to process`);

        // Step 2: Prepare bulk operations
        const contentPackagesBulkOps = [];
        const pricingContentPackagesBulkOps = [];

        for (const contentPackageGroup of allContentPackageGroups) {
          const { sequence_id, content_block_definitions } = contentPackageGroup;

          for (const { content_block_id, version } of content_block_definitions) {
            contentPackagesBulkOps.push({
              updateMany: {
                filter: {
                  'content_block._id': content_block_id,
                  'content_block.version': version,
                  $or: [
                    { sequence_id: { $exists: false } },
                    { sequence_id: null }
                  ]
                },
                update: { $set: { sequence_id } }
              }
            });

            pricingContentPackagesBulkOps.push({
              updateMany: {
                filter: {
                  'contents.content_blocks._id': content_block_id,
                  'contents.content_blocks.version': version.toString(),
                  $or: [
                    { sequence_id: { $exists: false } },
                    { sequence_id: null }
                  ]
                },
                update: [
                  {
                    $set: {
                      sequence_id: sequence_id,
                      'contents.content_blocks': {
                        $map: {
                          input: '$contents.content_blocks',
                          as: 'block',
                          in: {
                            $mergeObjects: [
                              '$$block',
                              {
                                $cond: {
                                  if: { $eq: ['$$block.name', 'price-entry-capability'] },
                                  then: { sequence_id: `${sequence_id}-prcent` },
                                  else: {
                                    $cond: {
                                      if: { $eq: ['$$block.name', 'richtext-capability'] },
                                      then: { sequence_id: `${sequence_id}-rchtxt` },
                                      else: {}
                                    }
                                  }
                                }
                              }
                            ]
                          }
                        }
                      }
                    }
                  }
                ]
              }
            });
          }
        }

        console.log(`Prepared ${contentPackagesBulkOps.length} bulk operations for content_packages`);
        console.log(`Prepared ${pricingContentPackagesBulkOps.length} bulk operations for pricing_content_packages`);

        // Step 3: Update content_packages & pricing_content_packages with sequence_ids
        if (contentPackagesBulkOps.length > 0) {
          const contentPackagesResult = await contentPackages.bulkWrite(contentPackagesBulkOps, { session });
          console.log(`Updated ${contentPackagesResult.modifiedCount} records in content_packages collection`);
        }

        if (pricingContentPackagesBulkOps.length > 0) {
          const pricingContentPackagesResult = await pricingContentPackages.bulkWrite(pricingContentPackagesBulkOps, { session });
          console.log(`Updated ${pricingContentPackagesResult.modifiedCount} records in pricing_content_packages collection`);
        }

        console.log('Migration completed - Updated content_packages & pricing_content_packages with sequence_ids.');
      });
    } finally {
      await session.endSession();
    }
  },

  async down(db, client) {
    throw new Error('Rollback unsupported');
  }
};