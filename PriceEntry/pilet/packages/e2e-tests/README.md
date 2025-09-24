# Price Entry-end-to-end (E2E) Automation Overview

## Techstack
- node.js
- playwright-1.42
- Editor: VS Code

This file provides an overview of the Price Entry E2E automation suite using playwright tool.

## Setup
Once you clone the pilet project in your local environment, use 'npm install' to download all the required dependencies for the project.

## Test Setup
Test Setup
Environment Configuration- Set the desired environment in which you want to run the tests inside the .env file. You can specify in which environment your tests should run(systest, staging). Also you can change/update username, password.

## Steps to run in local
Before running the tests for the first time be sure to run: ```npx playwright install``` in order to download the browser executable for playwright.
To run the price entry test suite from system, you can use the following command : 
- Run all tests in browser(headed) using following command from inside the project folder: ```npm run test:all```
- Run all tests in headless mode using following command from inside the project folder: ```npm run test:headless```
- Run only one test file in browser using following command from inside the project folder: ```npx playwright test testcasename --headed``` (Ex:  npx playwright test singlePriceEntry.spec.ts --headed)
- Run the test cases using UI mode : ```npx playwright test testcasename --ui```
- Debug all tests using following command from inside the project folder: ```npm run test:debug```

## Results
Once the test run is complete, a `playwright-report` folder will be generated under `~/packages/e2e-tests`. In case of test failures, screenshots will be captured and attached to the failure report.
`index.html` file will available in ./playwright-report folder.

## Useful document from Playwright
Please visit Playwright official website for more information: 'https://playwright.dev/'
