import { test as teardown, expect, Page } from '@playwright/test';
import { HomePage } from "../pages/homePage";
import { LoginPage } from "../pages/loginPage";
import { Helpers } from "./helper";
import credentials from "../constants/credentials";
const helper = new Helpers();
const today: Date = new Date();
const currentDay = today.getDate();
const month = helper.getMonth();
let pageId;

teardown('clean up - delete the page', async ({ page }) => {
  const homePage = new HomePage(page);
  const loginPage = new LoginPage(page);
  await loginPage.login_PriceEntry(credentials.chaUser.username, credentials.chaUser.password);
  await expect(homePage.createNewPageButton).toHaveAccessibleName("Create New");
  await page
    .locator("a[data-testid='page-title-link']", {
      hasText: "Price entry E2E-Automation" + "-" + currentDay + "-" + month,
    })
    .click();
  // Find the page with the specific page ID
  pageId = await homePage.fetchPageId();
  await page.goBack();
  await homePage.fetchPageByPageId("content/pricing", pageId);
  // Find the delete button within that row
  const deleteButton = page.locator(`[data-testid="delete-page-${pageId}"]`);
  if (await deleteButton.count() > 0) {
    await deleteButton.click();
  } else {
    console.log("Delete button not found for the page.");
  }
  await expect(page.getByTestId("delete-page-modal")).toBeVisible();
  await page.getByTestId("modal-footer-delete-btn").click();
  const deleteResponse = await page.waitForResponse((req) => {
    const request = req.request();
    const requestBody = request.postDataJSON();
    return (
      request.url().includes("/api/canvas/v1/graphql") &&
      requestBody.operationName === "deleteCanvasPage"
    );
  });
});

