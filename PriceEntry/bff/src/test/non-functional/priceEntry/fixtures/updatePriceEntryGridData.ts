// GraphQL mutation for updating price entry grid data
// Requires a priceItemInput object as variable
export const updatePriceDataQuery = {
    query: `
        mutation updatePriceEntryGridData($priceItemInput: PriceItemInput!) {
            updatePriceEntryGridData(priceItemInput: $priceItemInput) {
                id
                __typename
            }
        }
    `,
};