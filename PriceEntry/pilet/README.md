# Price Entry Pilet

> Micro frontend for price entry.

## Local Setup
 

### Initializing Secrets
A one-time addition of environment variables is required:

```
setx MONGO_DB_USER "required_mongodb_username"
setx MONGO_DB_PWD "required_mongodb_password"
setx FEED_SERVICE_API_KEY "required_feed_service_api_key"
setx PRICE_ENTRY_MONGO_CONNECTION_STRING "required_connection_string"
setx PERIOD_GENERATOR_MONGO_CONNECTION_STRING "required_connection_string"
setx RICH_TEXT_MONGO_CONNECTION_STRING "required_connection_string"
```

### Run Yarn Install
When cloning for the first time, the package dependencies need to be installed:
- Change directory to `packages/price-entry-pilet`
- Run the following command:
  ```
  yarn install --network-timeout 100000
  ```

### Running Authoring Locally with Mock BFF


- Change directory to `packages/price-entry-pilet`
- Start new core bff mock. This is required for the user preferences call the appshell makes
  ```
  yarn start:app-shell-mock
  ```
- Change directory to `packages/bff-mock-ts`
- Start the price entry bff mock. This is for both authoring and subscriber.
  ```
  yarn start:mock
  ```
- Change directory to `packages/price-entry-pilet`
  - For authoring:
    - Start the pilet in **authoring** mode: `yarn start:authoring:mock`
    - Go to `https://localhost:8083/home/mfe/price-entry`
  - For subscriber:
    - Start the pilet in **subscriber** mode: `yarn start:subscriber:mock`
    - Go to `https://localhost:8083/home/mfe/price-entry`

### Running Authoring Locally with Docker

#### Running All Dependencies

- Change directory to the root of this project
- Use the command 
  ```
  ./dockerup.ps1
  ```
- Change directory to `packages/price-entry-pilet`
- Start pilet in **authoring mode** with the feed service (for loading the Rich Text capability):
  ```
  yarn start:authoring:feed
  ```
 - Go to https://localhost:8050/home/mfe/price-entry/authoring and use the valid credentials


#### Running With Mock App Shell

- Change directory to the root of this project
- Use the command 
  ```
  ./dockerup.ps1 -UseMockAppShell
  ```
- Change directory to `packages/price-entry-pilet`
- Start pilet in **authoring mode**:
  ```
  yarn start:authoring
  ```
 - Go to https://localhost:8050/home/mfe/price-entry/authoring and use the valid credentials
 - This will automatically omit the Rich Text capability as the Mock App Shell does not include the correct roles for running this.

#### Omit Rich Text Capability

- Change directory to the root of this project
- Use the command 
  ```
  ./dockerup.ps1 -OmitRichText
  ```
- Change directory to `packages/price-entry-pilet`
- Start pilet in **authoring mode**:
  ```
  yarn start:authoring
  ```
 - Go to https://localhost:8050/home/mfe/price-entry/authoring and use the valid credentials
 - This will omit the Rich Text capability to reduce the number of running dependent containers.


## Packages

The main components of this repo are as follows:

### Pilet price-entry Package

`~/packages/price-entry-pilet`
This is an Pilet react project with the following already setup.

- Integration with the app shell
- Examples of registering components with piral in various ways
- Unit tests
- Some standard utilities and components that will be useful when starting a new project.

### BFF Mock (Typescript version)

`~/packages/bff-mock-ts`
Provides a BFF mock (Typescript) for the app-shell's bff and a simple one for the pilet-price-entry project with a hello world endpoint.
Running a pilet launches the app shell with it. Because of this, we either need to have a BFF mock running for the app-shell's (core) BFF or the actual core BFF in a docker container when debugging.

> Provides a mock appshell bff to not use docker resources to run app shell