# supply-and-demand Pilet
This is the repo for supply-and-demand pilet.

To get up and running quickly a quick getting started guide refer to https://piral.docs.cha.eu-west-1.dev/


## Running locally

- Download local certificiates 
  ```
  docker-compose -f docker-compose-certificates.yml up -d

  ```

- Change directory to `packages/supply-and-demand-pilet`

- Start new core bff mock. This is required for the preferences call the appshell makes
  ```
  yarn start:app-shell-mock
  ```
- Start pilet. This will open frontend in the browser
  ```
  yarn start:mock
  ```

## Packages

The main components of this repo are as follows:

### Pilet supply-and-demand Package
`~/packages/supply-and-demand-pilet`
This is an Pilet react project with the following already setup.
- Integration with the app shell
- Examples of registering components with piral in various ways
- Unit tests
- Some standard utilities and components that will be useful when starting a new project.

### BFF Mock
`~/packages/bff-mock`
Provides a BFF mock for the app-shell's bff and a simple one for the pilet-supply-and-demand project with a hello world endpoint.
Running a pilet launches the app shell with it. Because of this, we either need to have a BFF mock running for the app-shell's (core) BFF or the actual core BFF in a docker container when debugging.

### BFF Mock (Typescript version)
`~/packages/bff-mock-ts`
Provides a BFF mock (Typescript) for the app-shell's bff and a simple one for the pilet-supply-and-demand project with a hello world endpoint.
Running a pilet launches the app shell with it. Because of this, we either need to have a BFF mock running for the app-shell's (core) BFF or the actual core BFF in a docker container when debugging.
> Provides a mock appshell bff to not use docker resources to run app shell

## Docker Setup
`~/dockerup.sh` and `docker-compose.yml`
By default this provides the stack that's required to run the app shell. If you have a BFF you need to run for your pilet then this should be added to these two files. Comments have been added to show where your BFF should be added.

## TODO: (help welcome!)

- Ensure pilet starter pipeline generator command works
- Ensure build is passing
- Update starter to use standard folders for assets in e2e tests
- Update app shell to match
- Update pilet-starter to minimise code changes needed for forks
- Add docs covering CI
