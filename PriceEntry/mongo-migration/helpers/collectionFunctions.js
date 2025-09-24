async function ensureCollectionExists(db, collectionName) {
  const collections = await db.listCollections().toArray();
  const collectionExists = collections.some((c) => c.name === collectionName);
  if (!collectionExists) {
    await db.createCollection(collectionName);
  }
}

async function dropIfExists(db, collectionName) {
  const collections = await db.listCollections().toArray();
  const collectionExists = collections.some((c) => c.name === collectionName);
  if (collectionExists) {
    await db.collection(collectionName).drop();
  } else {
    console.log(`${collectionName} not found.`);
  }
}

module.exports = {
  ensureCollectionExists,
  dropIfExists,
};
