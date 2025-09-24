import delayResponse from "../utils/delayResponse";
import validateEntitlements from "../utils/validateEntitlements";
import { productData } from "../mock-data/productData";

export const definitions = `
  type Product {
    code: String,
    description: String,
    id: Int,
  }
`;

export const query = `products: [Product]`;


export const resolvers = {
    queries: {
      products: (_: any, args: any) => delayResponse(validateEntitlements(args, productData), 1500),
    },
  };

