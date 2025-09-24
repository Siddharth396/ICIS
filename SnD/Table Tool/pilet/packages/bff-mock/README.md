# Backend For Frontend Mock Server

This is the mock api for the example pilet.
It also runs a simple mock for the app shell preferences query to allow the app shell to start.

## Setup and Installation

* Run ` docker-compose -f docker-compose-certificates.yml up -d` in the base directory to inject local certificates.
* Install the dependencies by running `yarn install` in this directory.
* Start the BFF mock server by running `yarn start` in this directory.
