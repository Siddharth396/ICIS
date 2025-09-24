import delayResponse from "../utils/delayResponse";
import validateEntitlements from "../utils/validateEntitlements";

interface Capacity {
  country: string,
  company: string,
  site: string,
  plantNo: number,
  type: string,
  estimatedStart: string,
  newAnnualCapacity: number,
  capacityChange: number,
  percentChange: number,
  lastUpdated: string
}

export const definitions = `
  type Capacity {
    country: String,
    company: String,
    site: String,
    plantNo: Int,
    type: String,
    estimatedStart: String,
    newAnnualCapacity: Int,
    capacityChange: Int,
    percentChange: Int,
    lastUpdated: String
  }
`;

export const query = `capacityDevelopmentsByCommoditiesAndRegions(commodities: String!, regions: String!): [Capacity]`;

const populateData = (args: any) => {
  let capacity: Capacity[] = [];
  for(let i = 0; i < 15; i++)
    capacity.push({
      country: 'Country',
      company: 'Company',
      site: 'Site',
      plantNo: 1,
      type: 'Type',
      estimatedStart: '2021-01-01',
      newAnnualCapacity: 1000,
      capacityChange: 100,
      percentChange: 10,
      lastUpdated: '2021-01-01'
    }); 
  return capacity;
};

export const resolvers = {
  queries: {
    capacityDevelopmentsByCommoditiesAndRegions: (_: any, args: any) => delayResponse(validateEntitlements(args, populateData(args)), 1500),
  },
};

