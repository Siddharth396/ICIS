import delayResponse from "../utils/delayResponse";
import validateEntitlements from "../utils/validateEntitlements";
import { regionData } from "../mock-data/regionData";

export const definitions = `
  type Region {
    code: String,
    description: String,
    id: Int,
  }
`;

export const query = `regions: [Region]`;

export const resolvers = {
    queries: {
        regions: (_: any, args: any) => delayResponse(validateEntitlements(args, regionData), 1500),
    },
  };

