# BFF Starter
This is a starter code for a BFF which contains the basic skeleton code to enable quick start for creating authoring and subscriber bff.

## Customisation
To customize this for your project you can makes changes in the following:

- Run unit tests and update if one fails
- Update database name in 
  - [tests](./src/test/unit/Bff.Tests/Stubs/MongoDbService.cs#17)
  - [.env](./src/main/Authoring/.env)
  - [.env](./src/main/Subscriber/.env)
  - [docker-compose.yml](./docker-compose.yml)
- Review [Mocks folder](./src/test/unit/Bff.Tests/Infrastructure/Mocks) and remove/change depending if you are making HTTP REST calls to those services
- Update [Namespaces](./src/test/unit/Bff.Tests/Infrastructure/Common/Namespaces.cs) in case you change the default namespaces
- Update nuget packages, especially those created by ICIS squads and stored in nuget here https://artifacts.cha.rbxd.ds

## Examples
This projects comes with some examples of how to store and retrieve data to and from MongoDB using GraphQL. The examples can be found in [Application->Domain](./src/main/Authoring/Application/Domain) folder and the unit tests in the corresponding folder.

## Caution
`HOST_URLS` and `ports` can be a little bit confusing. You could see that there are different values for the port on which this BFF runs depending where you look at: [.env](./src/main/Authoring/.env#3), [.env](./src/main/Subscriber/.env#3), [docker-compose.yml](./docker-compose.yml#58), [docker-compose.yml](./docker-compose.yml#109), [launchSettings.json](src\main\Authoring\Properties\launchSettings.json#7), [launchSettings.json](src\main\Subscriber\Properties\launchSettings.json#7) and [Dockerfile](./src/main/docker/Dockerfile#61,65). For now, as it is setup it will run local Authoring bff on port `8005` and Subscriber bff on port `8003` when using ```dotnet run``` and from `docker-compose`. Port `80` is used only inside the container when running on any pre-live environment (systest, staging, integration, performance) and on live.

## Useful links
- [Steps to create a new BFF and deploy on k8s](https://confluence.tools.cha.rbxd.ds/display/GEN/Steps+to+create+a+new+BFF+and+deploy+on+k8s)
- [Creating a new environment and deploy an application for the first time](https://confluence.tools.cha.rbxd.ds/display/SE/Creating+a+new+environment+and+deploying+an+application+for+the+first+time)

