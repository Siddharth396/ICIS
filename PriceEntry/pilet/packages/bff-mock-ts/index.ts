import "./env";
import { join } from "path";
import express from "express";
import { readFileSync } from "fs";
import https from "https";
import { ApolloServer, gql } from "apollo-server-express";
import { applyString, applyObject, ISchema } from "./utils/applySchema";
import * as priceEntrySchema from "./schemas";

const schemas: ISchema[] = [priceEntrySchema];

// Paths to SSL certificate and key
const certPath = join(__dirname, "./cert.pem");
const keyPath = join(__dirname, "./key.pem");
const cert = readFileSync(certPath);
const key = readFileSync(keyPath);

const startBff = (port: string) => {
  const app = express();

  app.use("/test", (req, res) => {
    res.send('{"version":"mock-bff","product":"test.Mock.BFF"}');
  });

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
    scalar Date
    scalar DateTime
    scalar Decimal

    ${applyString(schemas)("definitions")}

    type Query {
      ${applyString(schemas)("query")}
    }

    type Mutation {
      ${applyString(schemas)("mutation")}
    }
  `;

  const resolvers = {
    Query: applyObject(schemas)("queries"),
    Mutation: applyObject(schemas)("mutations"),
  };

  // Create Apollo Server
  const server = new ApolloServer({
    typeDefs,
    resolvers,
  });

  // Start Apollo Server and apply middleware
  server.start().then(() => {
    server.applyMiddleware({
      app,
      path: process.env.MOCK_BFF_GRAPHQL_PATH || "/graphql",
    });

    // Create HTTPS server
    const httpsServer = https.createServer({ key, cert }, app);

    // Start listening
    httpsServer.listen(parseInt(port, 10), () => {
      console.log(`🚀 Server ready at https://localhost:${port}/${server.graphqlPath}`);
    });
  });
};

const port1 = process.env.MOCK_BFF_PORT;
const port2 = process.env.MOCK_AUTHORING_BFF_PORT;

if (!port1 || !port2) {
  throw new Error(
    "Environment variables MOCK_BFF_PORT and MOCK_AUTHORING_BFF_PORT must be defined"
  );
}

startBff(port1); // subscriber BFF
startBff(port2); // authoring BFF
