import "./env";

import { join } from "path";
import BFF from "@icis/bff-mock";
import { applyString, applyObject, ISchema } from "./utils/applySchema";
import * as capacities from "./schemas/capacities";
import * as outages from "./schemas/outages";
import * as contentBlock from "./schemas/contentBlock";
import * as regions from "./schemas/regions";
import * as products from "./schemas/products";
import * as certPaths from "./utils/certPaths";

const schemas: ISchema[] = [
  capacities,
  outages,
  regions,
  products,
  contentBlock
];

const startBff = (port: string) => {
  const bff = new BFF(certPaths.certFilePath, certPaths.certKeyFilePath, {
    //@ts-ignore
    version: process.env.MOCK_BFF_VERSION,
    //@ts-ignore
    port: parseInt(port, 10),
    graphQlEndpointPath: process.env.MOCK_BFF_GRAPHQL_PATH,
    graphiQLEndpointPath: process.env.MOCK_BFF_GRAPHIQL_PATH,
  });

  bff.app.use('/test', (req: any, res: any) => {
    res.send('{"version":"mock-bff","product":"test.Mock.BFF"}');
  });

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
}

startBff(process.env.MOCK_BFF_PORT); // subscriber BFF
startBff(process.env.MOCK_AUTHORING_BFF_PORT); // authoring BFF
