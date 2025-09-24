import { writeFileSync } from "fs";
import { join } from "path";
import { printSchema, getIntrospectionQuery, buildClientSchema } from "graphql";
import fetch from "node-fetch";

// Fetch the last argument as the GraphQL endpoint URL
const graphQLUrl = process.argv[process.argv.length - 1];
const outputPath = join(__dirname, "../schema.json");

(async () => {
  try {
    const introspectionQuery = getIntrospectionQuery();
    const response = await fetch(graphQLUrl, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query: introspectionQuery }),
    });
    const result = await response.json();

    if (result.errors) {
      console.error("Errors in introspection query:", result.errors);
      process.exit(1);
    } else {
      const schema = buildClientSchema(result.data);
      const sdl = printSchema(schema);
      writeFileSync(outputPath, sdl);
      console.log(`Schema has been exported to ${outputPath}`);
    }
  } catch (error) {
    console.error("Error fetching schema:", error);
    process.exit(1);
  }
})();
