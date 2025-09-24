import "./env";

import { join } from "path";
import BFF from "@icis/bff-mock";
import { applyString, applyObject, ISchema } from "./utils/applySchema";
import * as userPreferences from "./schemas/appShellUserPreferences";
import * as capacities from "./schemas/capacities";
import * as outages from "./schemas/outages";
import * as contentBlock from "./schemas/contentBlock";
import * as certPaths from "./utils/certPaths";

/**
 * Add all imported schemas here, they will get auto applied below.
 */
const schemas: ISchema[] = [
  userPreferences,
  capacities,
  outages,
  contentBlock
];

const bff = new BFF(
  certPaths.certFilePath,
  certPaths.certKeyFilePath,
  {
    //@ts-ignore
    version: process.env.MOCK_BFF_VERSION,
    //@ts-ignore
    port: 4200,
    graphQlEndpointPath: process.env.MOCK_BFF_GRAPHQL_PATH,
    graphiQLEndpointPath: process.env.MOCK_BFF_GRAPHIQL_PATH,
  }
);

bff.app.use((req: any, resp: any, next: any) => {
  // Decide on what makes a user not authorized here.
  const isAuthorized = true;

  if (isAuthorized) {
    next();
  } else {
    resp.status(403);
    resp.send();
  }
});
bff.useBasicGraphQLExpress(
  [
    {
      definition: `
        scalar UnixMillisTimestamp
        scalar ID
        scalar Long
        scalar ObjectId

        ${applyString(schemas)("definitions")}
      `,
      query: applyString(schemas)("query"),
      mutation: applyString(schemas)("mutation")
    },
  ],
  {
    //@ts-ignore
    queries: applyObject(schemas)("queries"),
    mutations: applyObject(schemas)("mutations")
  }
);

bff.serve();

export default bff;
