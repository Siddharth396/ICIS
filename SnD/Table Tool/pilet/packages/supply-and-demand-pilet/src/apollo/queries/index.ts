import gql from 'graphql-tag';

export const FETCH_CAPACITY = gql`
  query capacityDevelopmentsByCommoditiesAndRegions(
    $commodities: String!
    $regions: String!
  ) {
    capacityDevelopmentsByCommoditiesAndRegions(
      commodities: $commodities
      regions: $regions
    ) {
      country
      company
      site
      plantNo
      type
      estimatedStart
      newAnnualCapacity
      capacityChange
      percentChange
      lastUpdated
    }
  }
`;

export const FETCH_CONTENTBLOCK = gql`
  query contentBlock(
    $contentBlockId: String!
    $version: String!
  ) {
    contentBlock(contentBlockRequest: {
        contentBlockId: $contentBlockId,
        version: $version
    }) {
        filter
   }
  }
`;

export const SAVE_CONTENTBLOCK = gql`
  mutation saveContentBlock(
    $contentBlockId: String!
    $filter: String!
  )  {
    saveContentBlock(contentBlockRequest: {
        contentBlockId: $contentBlockId,
        filter: $filter
    }) {
        contentBlockId,
        version
   }
  }
`;

export const FETCH_OUTAGE = gql`
  query outagesByCommoditiesAndRegions(
    $commodities: String!
    $regions: String!
  ) {
    outagesByCommoditiesAndRegions(
      commodities: $commodities
      regions: $regions
    ) {
      outageStart
      outageEnd
      country
      company
      site
      plantNo
      cause
      capacityLoss
      totalAnnualCapacity
      lastUpdated
      comments
    }
  }
`;

export const FETCH_REGIONS = gql`
  query regions{
    regions {
      code
      description
      id
    }
  }
`;

export const FETCH_PRODUCT = gql`
  query products {
    products {
      code
      description
      id
    }
  }
`;
