import populateData from "../mock-data/contentBlock";
import delayResponse from "../utils/delayResponse";
import validateEntitlements from "../utils/validateEntitlements";

export const definitions = `
  type contentBlockMetadata {
    filter: String
  }

  type saveContentBlockResponse {
    contentBlockId: String,
    version: String
  }

   input ContentBlockRequest {
    contentBlockId: String,
    version: String
  }

   input SaveContentBlockRequest {
    contentBlockId: String,
    filter: String
  }
`;

export const query = `contentBlock(contentBlockRequest: ContentBlockRequest!): contentBlockMetadata`;
export const mutation = `saveContentBlock(contentBlockRequest: SaveContentBlockRequest!): saveContentBlockResponse`;


export const resolvers = {
  queries: {
    contentBlock: (_: any, args: any) => delayResponse(validateEntitlements(args, populateData.queryResponseData(args)), 1500)
  },
  mutations: {
    saveContentBlock: (_: any, args: any) => delayResponse(validateEntitlements(args, populateData.mutationResponseData(args)), 1500)
  }
};

