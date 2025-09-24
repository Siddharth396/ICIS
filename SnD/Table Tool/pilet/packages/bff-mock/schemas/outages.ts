import delayResponse from "../utils/delayResponse";
import validateEntitlements from "../utils/validateEntitlements";

interface Outage {
  outageStart: string,
  outageEnd: string,
  country: string,
  company: string,
  site: string,
  plantNo: number,
  cause: string,
  capacityLoss: string,
  totalAnnualCapacity: number,
  lastUpdated: string,
  comments: string,
}

export const definitions = `
  type Outage {
    outageStart: String,
    outageEnd: String,
    country: String,
    company: String,
    site: String,
    plantNo: Int,
    cause: String,
    capacityLoss: String,
    totalAnnualCapacity: Int,
    lastUpdated: String,
    comments: String,
  }
`;

export const query = `outagesByCommoditiesAndRegions(commodities: String!, regions: String!): [Outage]`;

const populateData = (args: any) => {
  let outages: Outage[] = [];
  for(let i = 0; i < 15; i++)
    outages.push({
      outageStart: '2021-01-01',
      outageEnd: '2021-01-02',
      country: 'Country',
      company: 'Company',
      site: 'Site',
      plantNo: 1,
      cause: 'Cause',
      capacityLoss: '1000',
      totalAnnualCapacity: 1000,
      lastUpdated: '2021-01-01',
      comments: 'Comments',
    }); 
  return outages;
};

export const resolvers = {
  queries: {
    outagesByCommoditiesAndRegions: (_: any, args: any) => delayResponse(validateEntitlements(args, populateData(args)), 1500),
  },
};

