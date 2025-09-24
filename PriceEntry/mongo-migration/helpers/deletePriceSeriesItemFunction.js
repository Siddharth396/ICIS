function validateDataPackagesForDeletion(dataPackages, seriesItemIds) {
    const deleteSeriesItemIdsSet = new Set(seriesItemIds.map(id => id.toString()));

    for (const dataPackage of dataPackages) {
        const allSeriesItemIds = (dataPackage.price_series_item_groups || [])
            .flatMap(group => group.price_series_item_ids || []);

        const otherSeriesItemIds = allSeriesItemIds.filter(id => !deleteSeriesItemIdsSet.has(id.toString()));

        if (otherSeriesItemIds.length > 0) {
            return {
                isValid: false,
                dataPackageId: dataPackage._id,
                otherSeriesItemIds: otherSeriesItemIds
            };
        }
    }

    return { isValid: true };
}

async function deletePriceSeriesItems(db, priceSeriesItemIds, session) {

    // Find all series items details based on _id or series_item_id
    const seriesItemsAggregationResult = await db.collection('price_series_items').find(
        {
            $or: [
                { _id: { $in: priceSeriesItemIds } },
                { series_item_id: { $in: priceSeriesItemIds } }
            ]
        },
        { session }
    ).toArray();

    const appliesFromDateTimes = seriesItemsAggregationResult.map((doc) => new Date(doc.applies_from_datetime));
    const seriesIds = seriesItemsAggregationResult.map((doc) => doc.series_id);
    const seriesItemIds = seriesItemsAggregationResult.map((doc) => doc._id);

    // Find all content block that belong to the series
    const contentBlocksAggregationResult = await db.collection('content_block_definitions').find(
        {
            'price_series_grids.price_series_ids': { $in: seriesIds }
        },
        { session }
    ).toArray();

    const contentBlockIds = contentBlocksAggregationResult.map((doc) => doc.content_block_id);

    // Find data package Id
    const dataPackageAggregationResult = await db.collection('content_packages').find(
        {
            'price_series_item_groups.price_series_item_ids': { $in: seriesItemIds },
        },
        { session }
    ).toArray();

    const dataPackageIds = dataPackageAggregationResult.map((doc) => doc._id);

    // Validate that data packages don't contain other series items
    const validationResult = validateDataPackagesForDeletion(dataPackageAggregationResult, seriesItemIds);
    if (!validationResult.isValid) {
        throw new Error(`Cannot delete data package ${validationResult.dataPackageId} because it contains other series items: ${validationResult.otherSeriesItemIds.join(', ')}`);
    }

    // Delete all price_series_items, content_packages, data_package_metadata, pricing_content_packages and commentaries
    await db.collection('price_series_items').deleteMany({ _id: { $in: seriesItemIds } },
        { session }
    );
    await db.collection('data_package_metadata').deleteMany({ _id: { $in: dataPackageIds } },
        { session }
    );
    await db.collection('content_packages').deleteMany({ _id: { $in: dataPackageIds } },
        { session }
    );
    await db.collection('pricing_content_packages').deleteMany({ _id: { $in: dataPackageIds } },
        { session }
    );

    await db.collection('commentaries').deleteMany({
        content_block_id: { $in: contentBlockIds },
        applies_from_datetime: { $in: appliesFromDateTimes }
    },
        { session }
    );
}

module.exports = {
    deletePriceSeriesItems,
};