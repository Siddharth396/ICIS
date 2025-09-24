const gridConfiguration = {
  columns: [
    {
      field: 'priceSeries',
      headerName: 'Price Series',
      cellDataType: 'text',
      values: null,
      pinned: 'left',
      displayOrder: 0,
    },
    {
      field: 'units',
      headerName: 'Units',
      cellDataType: 'text',
      values: null,
    },
    {
      field: 'status',
      headerName: 'Status',
      cellDataType: 'text',
    },
    {
      field: 'price',
      headerName: 'Price',
      tooltipField: 'price',
      cellDataType: 'number',
      cellType: 'input',
      values: null,
    },
    {
      field: 'dataUsed',
      headerName: 'Data Used',
      cellDataType: 'object',
      cellType: 'select',
      values: ['Bid/offer', 'Transaction', 'Spread', 'Fundamentals', 'Interpolation/extrapolation'],
    },
    {
      field: 'priceDelta',
      headerName: 'Change',
      cellDataType: 'priceDelta',
      values: null,
      customConfig: {
        priceDelta: {
          priceField: 'price',
          priceDeltaField: 'priceDelta',
          precisionField: 'valueDisplayPrecission',
        },
      },
    },
  ],
  __typename: 'GridConfiguration',
};

export const PriceSeriesData = [
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Netherlands_Month+1',
    validationErrors: { value: ['Error 1'] },

    name: [
      {
        value: 'LNG Spot DES Netherlands Month+1',
      },
    ],
    currency: {
      code: 'USD',
    },
    price: 10,
    priceDelta: 15,
    valueDisplayPrecission: 3,
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Netherlands_Month+2',
    name: [
      {
        value: 'LNG Spot DES Netherlands Month+2',
      },
    ],
    currency: {
      code: 'USD',
    },
    price: 10,
    status: 'IN DARFT',
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Britain_Month+1',
    name: [
      {
        value: 'LNG Spot DES Britain Month+1',
      },
    ],
    currency: {
      code: 'USD',
    },
    price: 10,
    priceDelta: 14,
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Britain_Month+2',
    name: [
      {
        value: 'LNG Spot DES Italy Month+2',
      },
    ],
    currency: {
      code: 'USD',
    },
    price: 10,
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Italy_Month+1',
    name: [
      {
        value: 'LNG Spot DES Italy Month+1',
      },
    ],
    currency: {
      code: 'USD',
    },
    price: 10,
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Italy_Month+2',
    name: [
      {
        value: 'LNG Spot DES Italy Month+2',
      },
    ],
    currency: {
      code: 'USD',
    },
    price: 10,
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Germany_Month+1',
    name: [
      {
        value: 'LNG Spot DES Germany Month+1',
      },
    ],
    currency: {
      code: 'USD',
    },
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_Germany_Month+2',
    name: [
      {
        value: 'LNG Spot DES Germany Month+2',
      },
    ],
    currency: {
      code: 'USD',
    },
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_France_Month+1',
    name: [
      {
        value: 'LNG Spot DES France Month+1',
      },
    ],
    currency: {
      code: 'USD',
    },
  },
  {
    seriesItemTypeCode: 'pi-single-with-ref',
    id: 'LNG_DES_France_Month+2',
    name: [
      {
        value: 'LNG Spot DES France Month+2',
      },
    ],
    currency: {
      code: 'USD',
    },
  },
];

export const ContentBlockData = {
  contentBlock: {
    title: 'test',
    contentBlockId: 'test-id',
    version: 1,
    priceSeriesGrids: [
      {
        id: 'mock-grid-id',
        title: 'Mock Grid 1',
        seriesItemTypeCode: 'pi-single-with-ref',
        gridConfiguration,
        priceSeries: [...PriceSeriesData],
        __typename: 'PriceSeriesGrid',
      },
    ],
    nextPublicationDate: {
      scheduledPublishDate: '2024-01-17T08:30:00.000Z',
      scheduleId: '397033db-b5af-4c9c-9bcb-03d1563ef67f',
    },
    nextActions: [{ name: 'PUBLISH', enabled: true }],
    commentary: {
      commentaryId: 'test-commentary-id',
      version: '0.1',
    },
    __typename: 'ContentBlockDefinition',
  },
};
