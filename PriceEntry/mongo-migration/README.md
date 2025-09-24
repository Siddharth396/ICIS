# Mongo DB Migrations

## How to create a migration file

There are 2 ways to create a migration file:
1. From your local computer
2. Using VS Code dev container feature

### From your local computer (recommended)
Follow the official documentation on how to install and use migrate-mongo https://github.com/seppevs/migrate-mongo#installation

Basically, you need to have node installed and to install migrate-mongo globally and then run the following command to create a new migration file:
```bash
migrate-mongo create <migration-name>
```

### Using VS Code dev container feature

If you are using VS Code, you can use the dev container feature to create a new migration file. Dev conatiners extend the VS Code remote development feature and allow you to develop inside a container. This is useful because you don't need to install anything on your local computer, everything is installed inside the container. Before you can use this feature, you need to install the Remote - Containers extension in VS Code.

Once you have the extension installed, you can open the command palette and type `Remote-Containers: Open Folder in Container...` and select the root folder of this project. This will open a new VS Code window inside a container. You can then open a new terminal and run the following command to create a new migration file:
```bash
migrate-mongo create <migration-name>
```
When you are using the dev container feature and you also run a local Mongo DB server using docker, the Mongo DB server will not be accessible from inside the dev container. The only way to test would be to build the docker image and run the migrations inside the container. To do this, run the following commands:
```bash
docker build -t mongo-migrations .
docker run --rm  --network mongodb_conatainer_netowrk_name --env-file .env mongo-migrations
```
Replace mongodb_conatainer_netowrk_name with the correct value: e.g. `local-development_default`

**The rest of the documentation will not be focused on this approach, but on the first one, the recommended one.**

## How to run migrations

Based on the official documentation, you can run the following command to run migrations:
```bash
migrate-mongo up
```

## Recommendations and best practices

1. Always create a new migration file for each change you want to make to the database. This way, you can easily revert the changes if something goes wrong.
2. Always test the migration on a local database before running it on production. This way, you can make sure that the migration is working as expected.
3. Always run the migrations in the correct order. The migration files are prefixed with a timestamp, so they are sorted by the timestamp. If you want to run a migration before another one, you can rename the file and add a timestamp that is lower than the other one.
4. Use the `migrate-mongo create` command to create a new migration file. This way, you can be sure that the file is created in the correct folder and that it has the correct name. If you create the file manually, you might forget to add the timestamp or to add the file in the correct folder. The `migrate-mongo create` command will use the `sample-migration.js` file as a template, and this will make sure that the file has the correct structure and that transactions are used. There might be operations that are not supported inside a transaction, so make sure to read the official documentation https://docs.mongodb.com/manual/core/transactions/.
5. DO NOT catch exceptions inside the migration file. If an exception is thrown, the migration will fail and the changes will not be applied. This is the expected behavior. If you catch the exception, the migration will not fail and the changes will NOT be applied, but the migration will be marked as `migrated`.
6. When creating a new migration DO NOT implement the `down` function as we do not support reverting migrations. If you want to revert a migration, you need to create a new migration that will revert the changes made by the previous migration.



## Advance use cases
For more advance use cases on how to use the migrate-mongo library, please visit the official documentation [migrate-mongo](https://github.com/seppevs/migrate-mongo?tab=readme-ov-file#advanced-features)

