import { defineConfig, devices } from "@playwright/test";
import appConfig from "./config/appConfig";
const authoringUrl = `${appConfig.authoringUrl}`;
/**
 * Read environment variables from file.
 * https://github.com/motdotla/dotenv
 */
// require('dotenv').config();

/**
 * See https://playwright.dev/docs/test-configuration.
 */
export default defineConfig({
  testDir: "./tests",
  timeout: 60 * 1000,
  expect: {
    /**
     * Maximum time expect() should wait for the condition to be met.
     * For example in `await expect(locator).toHaveText();`
     */
    timeout: 40000,
  },
  /* Run tests in files in parallel */
  fullyParallel: true,
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  //forbidOnly: !!process.env.CI,
  /* Retry on CI only */
  retries: process.env.CI ? 2 : 0,
  /* Opt out of parallel tests on CI. */
  workers: process.env.CI ? 1 : 1,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: [
    [
      "html",
      {
        open: "never",
        host: "0.0.0.0",
        port: 9223,
      },
    ],
    ["junit", { outputFile: "results.xml" }],
  ],
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    /* Base URL to use in actions like `await page.goto('/')`. */
    baseURL: authoringUrl,
    // Capture screenshot after each test failure.
    screenshot: "only-on-failure",

    /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: "on",
    ignoreHTTPSErrors: true,
    //storageState: './config/LoginAuth.json',
  },

  /* Configure projects for major browsers */
  projects: [
    {
      name: "setup",
      testDir: "./utilities",
      testMatch: "auth-setup.ts",
      use: {
        ignoreHTTPSErrors: true,
      },
      teardown: "cleanup",
    },
    {
      name: "cleanup",
      testDir: "./utilities",
      testMatch: "teardown.ts",
    },
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"],
        channel: 'chromium',
      viewport: { width: 1920, height: 1032 },
      storageState: './config/LoginAuth.json' },
      dependencies: ['setup']
    },
    // {
    //   name: "firefox",
    //   use: { ...devices["Desktop Firefox"],
    //     viewport: { width: 1920, height: 1080 },
    //   storageState: "./config/LoginAuth.json" },
    //   dependencies: ["setup"],
    // },

    // {
    //   name: 'webkit',
    //   use: { ...devices['Desktop Safari'],
    //     viewport: { width: 1920, height: 1080 },
    //   storageState: './config/LoginAuth.json' },
    //   dependencies: ['setup']
    //    },

    /* Test against mobile viewports. */
    // {
    //   name: 'Mobile Chrome',
    //   use: { ...devices['Pixel 5'] },
    // },
    // {
    //   name: 'Mobile Safari',
    //   use: { ...devices['iPhone 12'] },
    // },

    /* Test against branded browsers. */
    // {
    //   name: 'Microsoft Edge',
    //   use: { ...devices['Desktop Edge'], channel: 'msedge' },
    // },
  ],

  /* Run your local dev server before starting the tests */
  // webServer: {
  //   command: 'npm run start',
  //   url: 'http://127.0.0.1:3000',
  //   reuseExistingServer: !process.env.CI,
  // },
});
