<br />
<a name="readme-top"></a>
<div align="center">
  <h3 align="center">Price Entry BFF K6 Tests</h3>

  <p align="center">
    K6 API Testing Suite for the Price Entry BFF
  </p>


<br />
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li>
            <a href="#running-locally">Running Locally</a>
            <ul>
                <li><a href="#prerequisites-local">Prerequisites</a></li>
                <li><a href="#run-locally">Run Locally</a></li>
            <ul>
            <a href="#run-on-gitlab-on-different-environments">Run on Gitlab on different environments</a>
            </ul>
            </ul>
        </li>
      </ul>
    </li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## About The Project

This project is an API Automation Testing Suite using K6 as an API Load Testing tool to test Price Entry BFF

| Environment | API EndPoint |
| ----------- | -------------- |
| Systest     | https://authoring.systest.cha.rbxd.ds/api/price-entry/v1/graphql |
| Staging     | https://authoring.staging.cha.rbxd.ds/api/price-entry/v1/graphql |




<p align="right">(<a href="#readme-top">back to top</a>)</p>


### Built With

Frameworks:

* [![K6][K6]][K6-url]
* [![Docker][Docker]][Docker-url]


Languages:
* [![TypeScript][TypeScript]][TypeScript-url]




<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

### Running Locally

To run the Price Entry BFF k6 tests locally , please follow these steps:

##### Prerequisites (Local)

* NodeJS (version 16.20.1) and NPM
  * If you don't have NodeJS or NPM, please install it from [here](https://nodejs.org/en/download) (Version 16.20.1)
  * If you do have NodeJS and NPM, but it is a different version then NodeJS 16.20.1, please run the following **from an elevated command terminal**:
      ```sh
      // Install node Version Manager from Chocolatey
      choco install nvm
      ```

      ```sh
      // Install NodeJS version 16.20.1
      nvm install 16.20.1
      ```

      ```sh
      // Use NodeJS version 16.20.1
      nvm use 16.20.1
      ```


* [Yarn](https://classic.yarnpkg.com/lang/en/docs/install/#windows-stable)

  ```sh
  npm install yarn -g
  ```

##### Run Locally

Please run the following steps to run the K6 tests locally:

1. Clone the repo

   ```sh
   git clone https://github.com/LexisNexis-RBA/dsg-icis-capability-price-entry-bff.git
   ```

2. Switch to the specified directory   : cd dsg-icis-capability-price-entry-bff/src/test/non-functional/priceEntry

3. Install dependencies

   ```sh
   yarn install
   ```

4. Create a folder with the name 'html-results'

5. Run
  ```sh
   yarn bundle:start
   ```
6. The report will be created in the results folder named 'html-results'

<br />

### Run on Gitlab on different environments

To run the Price Entry BFF K6 Tests on Gitlab, please follow these steps:

1. Click on **Run pipeline**

2. Click **Run authoring-bff-systest-run-e2e-tests after deployment to systest**

<p align="right">(<a href="#readme-top">back to top</a>)</p>






[K6]:https://img.shields.io/badge/K6-1CA961?style=for-the-badge&logo=K6&logoColor=white
[K6-url]:https://k6.io/docs/
[Docker]: https://img.shields.io/static/v1?style=for-the-badge&message=Docker&color=2496ED&logo=Docker&logoColor=FFFFFF&label=
[Docker-url]: https://www.docker.com/

[TypeScript]: https://img.shields.io/badge/typescript-007ACC?style=for-the-badge&logo=typescript&logoColor=white
[TypeScript-url]: https://www.typescriptlang.org/


