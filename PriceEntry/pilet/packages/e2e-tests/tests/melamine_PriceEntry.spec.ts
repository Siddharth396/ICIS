import { test, expect, type Page } from "@playwright/test";
import { LoginPage } from "../pages/loginPage";
import { HomePage } from "../pages/homePage";
import { Helpers } from "../utilities/helper";
const testData = JSON.parse(JSON.stringify(require("../test-data/TestData.json")));
const helper = new Helpers();
const today: Date = new Date();
const currentDay = today.getDate();
const nextDay = helper.getNextDay();
const month = helper.getMonth();
const pageName = "Price entry E2E-Automation" + "-" + currentDay + "-" + month;

test.describe("Price Entry grid mandatory fields validations-Melamine", async () => {
  let loginPage: LoginPage;
  let homePage: HomePage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    homePage = new HomePage(page);
    await loginPage.goto();
    //Click on page created-Price entry E2E-Automation+current date+month
    await page
      .locator("a[data-testid='page-title-link']", {
        hasText: "Price entry E2E-Automation" + "-" + currentDay + "-" + month,
      })
      .click();
    await expect(homePage.pageTitle).toContainText(pageName);
    //Check if the grid is already available or not
    await homePage.checkForthePriceGridAvailablity();
  });
  test.afterEach(async ({ page }) => {
    //clean up the price grid
    await homePage.deletePriceGrid();
  });
  test(
    "Verify user is able to add data in the grid-Melamine",
    { tag: "@smoke" },
    async ({ page }) => {
      const gridTitle = "automation grid title";
      const [lowPriceValue] = helper.getRandomNumber();
      const highPriceValue = lowPriceValue + 1;
      //select filters and price series
      await homePage.selectSinglePriceSeries(
        "Melamine",
        "Asia-Pacific",
        "Assessed",
        "Spot",
        "Weekly",
        "Melamine  China  FOB 2-6 Weeks USD/MT Weekly"
      );
      await expect(homePage.grid_Load.last()).toBeVisible();
      //Enter grid title
      await homePage.enterGridTitle(gridTitle);
      //Select calender and select desired date
      await homePage.selectPreviousYearAndDate(currentDay, month);
      //Enter the low price value
      await homePage.enterLowPrice(lowPriceValue.toString());
      //Enter the high price value
      await homePage.enterHighPrice(highPriceValue.toString());
      //Verify the low price value is added in the grid
      await expect(homePage.lowPrice).toContainText(lowPriceValue.toString());
      //Verify the high price value is added in the grid
      await expect(homePage.highPrice).toContainText(highPriceValue.toString());
    }
  );
  test("Verify that when user add High and low price , Mid is correctly calculated for all the dates added", async ({
    page,
  }) => {
    const [lowPriceValue] = helper.getRandomNumber();
    const highPriceValue = lowPriceValue + 1;
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine  China  FOB 2-6 Weeks USD/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Enter High price and Low price

    await homePage.enterLowPrice(lowPriceValue.toString());
    await homePage.enterHighPrice(highPriceValue.toString());
    //Verify the mid price is corectly calculated
    const midPrice = helper.calc_midPrice(lowPriceValue, highPriceValue);
    const midPriceValue = String(midPrice);
    const midPrice_Text = await homePage.midPrice.first().textContent();
    expect(midPrice_Text).toContain(midPriceValue);
  });
  test("Validate Status tab functionality-Current Date - Melamine", async ({ page }) => {
    const [lowPriceValue] = helper.getRandomNumber();
    const highPriceValue = lowPriceValue + 1;
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Europe",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine European standard 99.8% Europe North West FD 4-6 Weeks EUR/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Update any editable column low price and high price
    //Enter High price and Low price
    await homePage.enterLowPrice(lowPriceValue.toString());
    await homePage.enterHighPrice(highPriceValue.toString());
    //Verify the "Status" column should change to "DRAFT"
    await expect(homePage.statusText).toContainText("DRAFT", { timeout: 10000 });
  });
  test("Validate Status tab functionality-Future Date - Melamine", async ({ page }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Europe",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine European standard 99.8% Europe North West FD 4-6 Weeks EUR/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select next day date
    await homePage.selectPreviousYearAndDate(nextDay, month);
    //Select next day date
    //await homePage.selectDate_Calender(nextDay, month);
    //Verify by default "Status" column should show the "READY TO START"
    await expect(homePage.statusText).toContainText("READY TO START");
  });
  test("Verify display of Price Commentary content block alongside each price entry grid-Melamine", async ({
    page,
  }) => {
    const sampleTestData_CurrentDate = "Sample test commentary for current day-Melamine";
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Europe",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine European standard 99.8% Europe North West FD 4-6 Weeks EUR/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Click on any other place to hide the Editor toolbar
    await homePage.click_Price_Series_Title.click();
    //Verify Price Commentary header and editor is visible
    await expect(homePage.priceCommentary_Heading).toHaveText("Price Commentary");
    await expect(homePage.priceCommentary_Editor).toBeVisible();
    //Add content in the Price Commentary editor
    await homePage.enterPriceCommentary(sampleTestData_CurrentDate);
    //Verify the content is added in the Price Commentary editor
    await expect(homePage.priceCommentary_Editor).toContainText(sampleTestData_CurrentDate);
  });
  test("Verify the Send for Review popup-without data", async ({ page }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Europe",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine European standard 99.8% Europe North West FD 4-6 Weeks EUR/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Click on any other place to hide the Editor toolbar
    await homePage.click_Price_Series_Title.click();
    //Verify the "Send for Review" button is visible
    await expect(homePage.workflowButton).toContainText("Send for review");
    //Verify the "Send for Review" popup when no data given
    await homePage.workflowButton.click();
    await homePage.verifySendForReviewPopup_withoutdata();
  });
  test("Verify the warning popup-without data", async ({ page }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine  Asia South East CFR 2-6 Weeks USD/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Click on any other place to hide the Editor toolbar
    await homePage.click_Price_Series_Title.click();
    //Verify the "Send for Review" button is visible
    await expect(homePage.workflowButton).toContainText("Send for review");
    //Verify the "Send for Review" warning popup when no data given
    await homePage.workflowButton.click();
    await homePage.verifySendForReviewWarningPopup_withoutdata();
  });
  test("Verify advance workflow for Melamine", async ({ page }) => {
    const [lowPriceValue] = helper.getRandomNumber();
    const highPriceValue = lowPriceValue + 1;
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine  Asia South East CFR 2-6 Weeks USD/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Verify the "Status" column should change to "READY TO START"
    await expect(homePage.statusText).toContainText("READY TO START", { timeout: 10000 });
    //Enter High price and Low price
    await homePage.enterLowPrice(lowPriceValue.toString());
    await homePage.enterHighPrice(highPriceValue.toString());
    //Verify the "Status" column should change to "DRAFT"
    await expect(homePage.statusText).toContainText("DRAFT", { timeout: 10000 });
    //Verify the "Send for Review" button is visible
    await expect(homePage.workflowButton).toContainText("Send for review");
    //Click on any other place to hide the Editor toolbar
    await homePage.click_Price_Series_Title.click();
    //Verify the "Send for Review" popup when no data given
    await homePage.workflowButton.click();
    await homePage.publishButton_modalPopup.click();
    await expect(homePage.workflowModalPopup).toContainText(
      "Prices have been successfully sent for review"
    );
    await homePage.closePopupModalWindow.click();
    await expect(homePage.workflowButton).toContainText("Pull back", { timeout: 10000 });
    //Verify the "Status" column should change to "READY FOR REVIEW"
    await expect(homePage.statusText).toContainText("READY FOR REVIEW");
    await homePage.reviewRequestLink.click({
      button: "right",
    });
    //verify the review request page
    await homePage.workFlow_ReviewPage();
    //Verify the "Status" column should change to "PUBLISHED"
    await expect(homePage.statusText).toContainText("PUBLISHED", { timeout: 10000 });
    await expect(page.getByTestId("workflow-button")).toContainText("Correction needed", {
      timeout: 10000,
    });
  });
  test("Verify pull back functionality-Melamine", async ({ page }) => {
    const [lowPriceValue] = helper.getRandomNumber();
    const highPriceValue = lowPriceValue + 1;
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Melamine",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Weekly",
      "Melamine  Asia South East CFR 2-6 Weeks USD/MT Weekly"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Verify the "Status" column should change to "READY TO START"
    await expect(homePage.statusText).toContainText("READY TO START", { timeout: 10000 });
    //Enter High price and Low price
    await homePage.enterLowPrice(lowPriceValue.toString());
    await homePage.enterHighPrice(highPriceValue.toString());
    //Verify the "Status" column should change to "DRAFT"
    await expect(homePage.statusText).toContainText("DRAFT", { timeout: 10000 });
    //Verify the "Send for Review" button is visible
    await expect(homePage.workflowButton).toContainText("Send for review");
    //Click on any other place to hide the Editor toolbar
    await homePage.click_Price_Series_Title.click();
    //Verify the "Send for Review" popup when no data given
    await homePage.workflowButton.click();
    await homePage.publishButton_modalPopup.click();
    await expect(homePage.workflowModalPopup).toContainText(
      "Prices have been successfully sent for review"
    );
    await homePage.closePopupModalWindow.click();
    await expect(homePage.workflowButton).toContainText("Pull back", { timeout: 10000 });
    //click on pull back button
    await homePage.workflowButton.click();
    await homePage.publishButton_modalPopup.click();
    await expect(homePage.workflowModalPopup).toContainText(
      "Content pulled back successfully"
    );
    await homePage.closePopupModalWindow.click();
    await expect(homePage.workflowButton).toContainText("Send for review", { timeout: 10000 });
  });
});
