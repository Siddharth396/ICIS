const { MongoClient } = require('mongodb');
const migration = require('../mongo-migration/migrations/20250703070903-add-assessed-datetime-and-applies-until-datetime.js');

async function run() {
  const uri = 'mongodb://localhost:27017/prcent-systest';
  const client = new MongoClient(uri);
  try {
    await client.connect();
    const db = client.db(); // default DB from URI
    await migration.up(db, client);
    console.log('Migration ran successfully');
  } catch (err) {
    console.error('Error running migration:', err);
  } finally {
    await client.close();
  }
}

run();
