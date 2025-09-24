// In this file you can configure migrate-mongo

const dotenv = require('dotenv');
const { checkEnvVariables } = require('./helpers/environment');

// With this configuration, dotenv will only load the values from .env
// if the variable is not already defined in the system environment.
// If the variable is already defined in the system environment,
// dotenv will use the value from the system environment instead.
// Note that this behavior is the default behavior of dotenv,
// so you don't need to set the override option to false if you want this behavior.
// You only need to set the override option to true if you want dotenv to override
// the values in the system environment with the values from .env.
dotenv.config({ override: false });

console.log('Initializing migrate-mongo-repeatable-config.js');

if (!checkEnvVariables(['MONGO_DB_USER', 'MONGO_DB_PWD', 'MONGO_DB_CONNECTION_STRING', 'MONGO_DB_NAME'])) {
  throw new Error('One or more required environment variables are missing');
}

const username = process.env.MONGO_DB_USER;
const password = process.env.MONGO_DB_PWD;
const connectionString = process.env.MONGO_DB_CONNECTION_STRING;
const dbName = process.env.MONGO_DB_NAME;

// replace <username> and <password> with your username and password
const connectionStringUrl = connectionString.replace('<username>', username).replace('<password>', password);

const config = {
  mongodb: {
    url: connectionStringUrl,
    databaseName: dbName,
    options: {
      //   connectTimeoutMS: 3600000, // increase connection timeout to 1 hour
      //   socketTimeoutMS: 3600000, // increase socket timeout to 1 hour
    },
  },

  // The migrations dir, can be an relative or absolute path.
  migrationsDir: 'repeatable_migrations',

  // The mongodb collection where the applied changes are stored.
  changelogCollectionName: 'migrations_changelog',

  // The file extension to create migrations and search for in migration dir
  migrationFileExtension: '.js',
  useFileHash: true,

  // Don't change this, unless you know what you're doing
  moduleSystem: 'commonjs',
};

module.exports = config;
