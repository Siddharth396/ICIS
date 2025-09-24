import { test, expect, type Page } from "@playwright/test";
import { LoginPage } from "../pages/loginPage";
import { HomePage } from "../pages/homePage";
import { Helpers } from "../utilities/helper";
const testData = JSON.parse(JSON.stringify(require("../test-data/TestData.json")));
const helper = new Helpers();
const today: Date = new Date();
const currentDay = today.getDate();
const month = helper.getMonth();
const pageName = "Price entry E2E-Automation" + "-" + currentDay + "-" + month;

test.describe("Price Entry grid mandatory fields validations-Styrene", async () => {
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
    "Verify user is able to add data in the grid-Styrene",
    { tag: "@smoke" },
    async ({ page }) => {
      const gridTitle = "automation grid title";
      const [lowPriceValue] = helper.getRandomNumber();
      const highPriceValue = lowPriceValue + 1;
      //Verify user is able to add data in the grid for Styrene commodity
      //select filters and price series
      await homePage.selectSinglePriceSeries(
        "Styrene", "Asia-Pacific", "Assessed", "Spot", "Daily",
        "Styrene China FOB 2-6 Weeks USD/MT Daily"
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
  test.skip("Verify single price value is available in styrene commodity", async ({ page }) => {
    //select filters and price series
    await homePage.selectOnlyCommadity("Styrene");
    //Verify single price value is available in styrene commodity
    await expect(homePage.singlePriceValue).toBeVisible();
    //check the single price value
    await homePage.singlePriceValue.click();
    await homePage.saveButton_modal_Popup.click();
    await expect(homePage.grid_Load).toBeVisible();
    //Verify Price grid should display selected single value price
    expect(homePage.singlePriceValue_Title_Grid).toBeTruthy();
  });
  test.skip("Verify single value price grid header titles", async ({ page }) => {
    //select filters and price series
    await homePage.selectOnlyCommadity("Styrene");
    //Verify single price value is available in styrene commodity
    await expect(homePage.singlePriceValue).toBeVisible();
    //check the single price value
    await homePage.singlePriceValue.click();
    await homePage.saveButton_modal_Popup.click();
    await expect(homePage.grid_Load).toBeVisible();
    //Verify single value price grid header titles
    const headerCount = homePage.header_Titles;
    for (let i = 0; i < (await headerCount.count()); i++) {
      await expect(homePage.header_Titles.nth(i)).toHaveText(
        testData.styrene_Signle_Value_Titles[i]
      );
    }
  });
  test.skip("Verify single value price grid data", async ({ page }) => {
    let [randomNumber] = helper.getRandomNumber();
    //select filters and price series
    await homePage.selectOnlyCommadity("Styrene");
    //Verify single price value is available in styrene commodity
    await expect(homePage.singlePriceValue).toBeVisible();
    //check the single price value
    await homePage.singlePriceValue.click();
    await homePage.saveButton_modal_Popup.click();
    await expect(homePage.grid_Load).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Enter the price value
    await homePage.enterPrice(randomNumber.toString());
    //Verify the single value price grid data
    await expect(homePage.priceField).toContainText(randomNumber.toString());
  });
  test("Verify display of Price Commentary content block alongside each price entry grid-Styrene", async ({
    page,
  }) => {
    const sampleTestData_CurrentDate = "Sample test commentary for current day-Styrene";
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "Styrene", "Asia-Pacific", "Assessed", "Spot", "Weekly",
      "Styrene South Korea FOB 3-6 Weeks Close USD/MT Weekly"
    );
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

});