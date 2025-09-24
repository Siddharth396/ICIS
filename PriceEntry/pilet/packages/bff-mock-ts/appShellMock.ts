import "./env";
import { join } from "path";
import express from "express";
import { readFileSync } from "fs";
import https from "https";
import { ApolloServer, gql } from "apollo-server-express";
import { applyString, applyObject, ISchema } from "./utils/applySchema";
import * as userPreferences from "./schemas/appShellUserPreferences";
import expressPlayground from "graphql-playground-middleware-express";

// Add all imported schemas here, they will get auto applied below.
const schemas: ISchema[] = [userPreferences];

// Paths to SSL certificate and key
const certPath = join(__dirname, "./cert.pem");
const keyPath = join(__dirname, "./key.pem");
const cert = readFileSync(certPath);
const key = readFileSync(keyPath);

// Create Express app
const app = express();

// Authorization middleware
app.use((req, res, next) => {
  // Decide on what makes a user not authorized here.
  const isAuthorized = true;

  if (isAuthorized) {
    next();
  } else {
    res.status(403).send();
  }
});

// Build GraphQL type definitions and resolvers
const typeDefs = gql`
  scalar UnixMillisTimestamp
  scalar ID
  scalar Long
  scalar ObjectId

  ${applyString(schemas)("definitions")}

  type Query {
    ${applyString(schemas)("query")}
  }
`;

const resolvers = {
  Query: applyObject(schemas)("queries"),
};

// Create Apollo Server
const server = new ApolloServer({
  typeDefs,
  resolvers,
});

// Start Apollo Server and apply middleware
(async () => {
  await server.start();
  server.applyMiddleware({
    app,
  });

  // Add GraphQL Playground middleware at the specified path
  const graphiqlPath = process.env.MOCK_BFF_GRAPHIQL_PATH || "/graphiql";
  const graphqlPath = process.env.MOCK_BFF_GRAPHQL_PATH || server.graphqlPath;

  app.get(
    graphiqlPath,
    expressPlayground({
      endpoint: graphqlPath,
    })
  );

  // Create HTTPS server
  const httpsServer = https.createServer({ key, cert }, app);

  // Start listening
  const port = process.env.PORT || 4200;
  httpsServer.listen(port, () => {
    console.log(`🚀 Server ready at https://localhost:${port}${server.graphqlPath}`);
    console.log(`🚀 GraphQL Playground available at https://localhost:${port}${graphiqlPath}`);
  });
})();

export default app;
