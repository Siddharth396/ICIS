import { test, expect, type Page } from "@playwright/test";
import { LoginPage } from "../pages/loginPage";
import { HomePage } from "../pages/homePage";
import { Helpers } from "../utilities/helper";
const testData = JSON.parse(JSON.stringify(require("../test-data/TestData.json")));
const helper = new Helpers();
const today: Date = new Date();
const currentDay = today.getDate();
const previousDay = helper.getPreviousDate();
const nextDay = helper.getNextDay();
const month = helper.getMonth();
const prevDateMonth = helper.getPreviousMonthOnly(today);
const pageName = "Price entry E2E-Automation" + "-" + currentDay + "-" + month;
test.describe("Price Entry grid mandatory fields validations-LNG", async () => {
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
  test("Verify Page loaded successfully", { tag: "@smoke" }, async ({ page }) => {
    const gridTitle = "automation grid title";
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Dubai DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Enter grid title
    await homePage.enterGridTitle(gridTitle);
  });
  test("Verify Grid loaded successfully", { tag: "@smoke" }, async ({ page }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Dubai DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Verify price grid loaded successfully and check the header titles
    const headerCount = homePage.header_Titles;
    for (let i = 0; i < (await headerCount.count()); i++) {
      await expect(homePage.header_Titles.nth(i)).toHaveText(testData.lng_Titles[i]);
    }
  });
  test("Verify user is able to add data in the grid-LNG", { tag: "@smoke" }, async ({ page }) => {
    const [randomNumber] = helper.getRandomNumber();
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Click Assessment method dropdown & Select field values
    await homePage.selectAssestMethod("Assessed");
    //Enter the price value
    await homePage.enterPrice(randomNumber.toString());
    //Verify the price value is added in the grid
    await expect(homePage.priceField).toContainText(randomNumber.toString());
  });
  test("Verify the price entry,data used field values are still the same when refresh the page-LNG", async ({
    page,
  }) => {
    let [randomNumber] = helper.getRandomNumber();
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Click Assessment method dropdown & Select field values
    await homePage.selectAssestMethod("Assessed");
    //Enter the price value
    await homePage.enterPrice(randomNumber.toString());
    //Click Data used dropdown & Select field values
    await homePage.selectDataUsed("Transaction");
    //refresh the page
    await page.reload();
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    await expect(homePage.priceField.first()).toContainText(randomNumber.toString());
    await expect(homePage.dataUsed_Field).toHaveText("Transaction");
  });
  test.skip("Verify that when user adds previous date data then for current date , last assessment is populated with the same value", async ({
    page,
  }) => {
    let [randomNumber] = helper.getRandomNumber();
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Singapore DES month +1 USD/MMBtu daily"
    );
    //Select Previous date
    await homePage.selectPreviousYearAndDate(previousDay, prevDateMonth);
    //Click Assessment method dropdown & Select field values
    await homePage.selectAssestMethod("Assessed");
    await homePage.enterPrice(randomNumber.toString());
    await expect(homePage.priceField.first()).toContainText(randomNumber.toString());
    //Select current date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    await expect(homePage.lastAssesmentPriceField).toContainText(randomNumber.toString());
  });
  test.skip("Verify that percentage change column reflects the correct value", async ({ page }) => {
    let [randomNumber1] = helper.getRandomNumber();
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    //Select Current date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    const lap = await homePage.lastAssesmentPriceField.textContent();
    const y = Number(lap);
    const price = randomNumber1;
    //Select Assesment method
    await homePage.selectAssestMethod("Assessed");
    // Enter Price value
    await homePage.enterPrice(randomNumber1.toString());
    const change = helper.calc_Percentage(price, y);
    const subValue = helper.calc_priceChange(price, y);
    console.log(subValue);
    const value = String(change);
    // get the change value
    const value_Text = await homePage.priceChange.first().textContent();
    console.log("Calculation: " + subValue + "" + "(" + value + ")");
    console.log("After calculation, change: " + value_Text);
    //validate the change value with percentage value
    expect(value_Text).toContain(+subValue + "" + "(" + value + ")");
  });
  test.skip("Validate Status tab functionality-Current Date - LNG", async ({ page }) => {
    let [randomNumber] = helper.getRandomNumber();
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    //Verify by default "Status" column should show the "READY TO START"
    await expect(homePage.statusText).toContainText("READY TO START");
    //Select Assesment method
    await homePage.selectAssestMethod("Assessed");
    //Update any editable column like, Price, Assessment method or Data used
    await homePage.enterPrice(randomNumber.toString());
    await expect(homePage.priceField.first()).toContainText(randomNumber.toString());
    //Verify the "Status" column should change to "DRAFT"
    await expect(homePage.statusText).toContainText("DRAFT", { timeout: 10000 });
  });
  test.skip("Validate Status tab functionality-Future Date - LNG", async ({ page }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    //Select next day date
    await homePage.selectPreviousYearAndDate(nextDay, month);
    //Verify the "Status" column should change to "IN DRAFT"
    await expect(homePage.statusText).toContainText("READY TO START");
  });
  test.skip("Validate that future values have persistest Assessment method value", async ({
    page,
  }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    //Select Assesment method
    await homePage.selectAssestMethod("Assessed");
    //Select next day date
    await homePage.selectPreviousYearAndDate(nextDay, month);
    //verify the Assessment method value
    await expect(homePage.assesMethod_Field).toContainText("Assessed");
  });
  test.skip("Validate that future date doesnot have validation errors on fields when status is ready to start", async ({
    page,
  }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    //Select next day date
    await homePage.selectPreviousYearAndDate(nextDay, month);
    //Verify by default "Status" column should show the "READY TO START"
    await expect(homePage.statusText).toContainText("READY TO START");
    //Verify error message for Price field
    const priceField = homePage.priceField;
    //Hover for price field
    await priceField.hover();
    //Verify error message for Price field
    await expect(homePage.toolTip_ErrorMessage.first()).toBeHidden();
    await page.waitForTimeout(500);
    const dataUsedField = homePage.dataUsed_Field;
    //Hover for data used field
    await dataUsedField.hover();
    //Verify error message for Price field
    await expect(homePage.toolTip_ErrorMessage.first()).toBeHidden();
  });
  test("Verify display of Price Commentary content block alongside each price entry grid-LNG", async ({
    page,
  }) => {
    const sampleTestData_CurrentDate = "Sample test commentary for current day-LNG";
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
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
  test("Verify current date, past date doesn't have the same commentary added-LNG", async ({
    page,
  }) => {
    const sampleTestData_CurrentDate = "Sample test automation commentary";
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Click on any other place to hide the Editor toolbar
    await homePage.click_Price_Series_Title.click();
    //Verify Price Commentary header and editor is visible
    await expect(homePage.priceCommentary_Heading).toHaveText("Price Commentary");
    await expect(homePage.priceCommentary_Editor).toBeVisible();
    //Add content in the Price Commentary editor
    const currentDate_Commentary =
      sampleTestData_CurrentDate + "-" + (await homePage.getDate_Fromcalendar());
    await homePage.enterPriceCommentary(currentDate_Commentary);
    //Verify the content is added in the Price Commentary editor
    await expect(homePage.priceCommentary_Editor).toContainText(currentDate_Commentary);
    //Select next day date
    await homePage.selectPreviousYearAndDate(previousDay, prevDateMonth);
    //Click on any other place to hide the Editor toolbar
    await homePage.click_Price_Series_Title.click();
    //Verify Price Commentary header and editor is visible
    await expect(homePage.priceCommentary_Heading).toHaveText("Price Commentary");
    await expect(homePage.priceCommentary_Editor).toBeVisible();
    //Add content in the Price Commentary editor
    const pastDate_Commentary =
      sampleTestData_CurrentDate + "-" + (await homePage.getDate_Fromcalendar());
    await homePage.enterPriceCommentary(pastDate_Commentary);
    //Verify the content is added in the Price Commentary editor
    await expect(homePage.priceCommentary_Editor).toContainText(pastDate_Commentary);
    //Verify the content is not same for current date and past date
    expect(currentDate_Commentary).not.toEqual(pastDate_Commentary);
  });
  test("Verify user should be able to access Show/Hide column tab.", async ({ page }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //verify the Show/Hide column tab
    await expect(homePage.showHide_Column_Tab).toBeVisible();
    //click on Show/Hide column tab
    await homePage.showHide_Column_Tab.click();
    //verify the Show/Hide column dropdown
    await expect(homePage.showHide_Column_Dropdown).toBeVisible();
  });
  test("Verify user should be able to select the non mandatory columns and verify the columns on grid which are marked as hidden-Show/Hide column tab.", async ({
    page,
  }) => {
    const multiple_Columns = ["Period", "Change", "Last assessment price"];
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //verify the Show/Hide column tab
    await expect(homePage.showHide_Column_Tab).toBeVisible();
    const headerTitles = homePage.header_Titles;
    console.log(
      "Header Titles before selecting non mandatory columns-" +
      (await headerTitles.allTextContents())
    );
    //select single column
    await homePage.click_NonManadatoryColumn("Status");
    const headerTitles_Column_Hidden = homePage.header_Titles;
    //verify the header titles count
    expect(headerTitles.count()).not.toEqual(headerTitles_Column_Hidden.count());
    console.log(
      "Columns on grid which is marked as hidden-" +
      (await headerTitles_Column_Hidden.allTextContents())
    );
    //Verify user should not see the columns on grid which are marked as hidden.-single
    expect(await headerTitles_Column_Hidden.allTextContents()).not.toContain("Status");
    //select multiple columns
    await homePage.click_NonManadatoryColumns_Multiple(multiple_Columns);
    console.log(
      "Columns on grid which are marked as hidden-" +
      (await headerTitles_Column_Hidden.allTextContents())
    );
    //Verify user should not see the columns on grid which are marked as hidden.-Multiple
    expect(await headerTitles_Column_Hidden.allTextContents()).not.toContain(multiple_Columns);
  });
  test("Verify user should see the columns on grid which are marked as show-Show/Hide column tab.", async ({
    page,
  }) => {
    const multiple_Columns = [
      "Period",
      "Unit",
      "Status",
      "Change",
      "Last assessment price",
      "Last assessment date",
    ];
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //verify the Show/Hide column tab
    await expect(homePage.showHide_Column_Tab).toBeVisible();
    const headerTitlesPresent: string = (await homePage.header_Titles.allTextContents()).join(", ");
    //select multiple columns in Show/Hide column tab
    await homePage.click_NonManadatoryColumns_Multiple(multiple_Columns);
    const headerTitles_Column_Hidden = homePage.header_Titles;
    console.log(
      "Columns on grid which are marked as hidden-" +
      (await headerTitles_Column_Hidden.allTextContents())
    );
    //Verify user should not see the columns on grid which are marked as hidden
    expect(await headerTitles_Column_Hidden.allTextContents()).not.toContain(multiple_Columns);
    await homePage.click_NonManadatoryColumns_Multiple(multiple_Columns);
    await page.waitForTimeout(500);
    console.log(
      "Columns on grid which are marked as show-" +
      (await headerTitles_Column_Hidden.allTextContents())
    );
    //Verify user should see the columns on grid which are marked as show
    for (let i = 0; i < (await headerTitles_Column_Hidden.count()); i++) {
      await expect(homePage.header_Titles.nth(i)).toHaveText(testData.lng_Titles[i]);
    }
  });
  test("Data validation on mandatory fields for LNG-Assessment method- Assessed-No input", async ({
    page,
  }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Select assessment method
    await homePage.selectAssestMethod("Assessed");
    //Clear the price field if any previous value is present
    await homePage.clearPriceInputField();
    //Verify error message for Price field
    const priceField = homePage.priceField;
    //Hover for price field
    await priceField.hover();
    //Verify error message for Price field
    await expect(homePage.toolTip_ErrorMessage.first()).toHaveText("Price is required.");
  });
  test("Data validation on mandatory fields for LNG-Assessment method- Assessed-Negative input", async ({
    page,
  }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Select assessment method
    await homePage.selectAssestMethod("Assessed");
    //Clear the price field if any previous value is present
    await homePage.clearPriceInputField();
    //Enter the price value
    await homePage.enterPrice("-12");
    await page.keyboard.down("Escape");
    //Verify error message for Price field
    const priceField = homePage.priceField;
    //Hover for price field
    await priceField.hover();
    //Verify error message for Price field
    await expect(homePage.toolTip_ErrorMessage.first()).toHaveText("Price must be greater than 0.");
  });
  test("Data validation on mandatory fields for LNG-Assessment method- Assessed-null input", async ({
    page,
  }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Select assessment method
    await homePage.selectAssestMethod("Assessed");
    //Clear the price field if any previous value is present
    await homePage.clearPriceInputField();
    //Enter the price value
    await homePage.enterPrice("0");
    await page.keyboard.down("Escape");
    //Verify error message for Price field
    const priceField = homePage.priceField;
    //Hover for price field
    await priceField.hover();
    //Verify error message for Price field
    await expect(homePage.toolTip_ErrorMessage.first()).toHaveText("Price must be greater than 0.");
  });
  test("Verify the reference price dropdown values in the price grid", async ({ page }) => {
    //select filters and price series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select calender and select desired date
    await homePage.selectPreviousYearAndDate(currentDay, month);
    //Select assessment method
    await homePage.selectAssestMethod("Premium/Discount");
    //Verify the reference price dropdown values
    await homePage.referencePriceDropdown.click();
    const referencePriceDropdownValues = homePage.referencePriceDropdownValues;
    for (let i = 0; i < (await referencePriceDropdownValues.count()); i++) {
      await expect(homePage.referencePriceDropdownValues.nth(i)).toHaveText(
        testData.referencePriceDropdownValues[i]
      );
    }
    await homePage.referencePriceDropdown.click();
  });
  test("Verify the reference price value for 'LNG DES Argentina' in the price grid", async ({
    page,
  }) => {
    let [randomNumber1, randomNumber2] = helper.getRandomNumber();//randomNumber1 generate integer value and randomNumber2 generate decimal value
    //Select DES series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Asia-Pacific",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Kuwait DES month +1 USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select assessment method as Premium/Discount
    await homePage.selectAssestMethod("Assessed");
    await homePage.selectAssestMethod("Premium/Discount");
    //Select reference price dropdown value
    await homePage.selectReferencePriceDropdownValue("LNG DES Argentina");
    //Enter Premium/Discount value
    await homePage.enterPremiumDiscountValue(randomNumber1.toString());
    //Select second price grid
    await homePage.addPriceEntryCapability();
    //Select LNG Argentina DES series
    await homePage.selectSinglePriceSeries(
      "LNG",
      "Americas",
      "Assessed",
      "Spot",
      "Daily",
      "LNG Argentina DES month +2 USD/MMBtu daily USD/MMBtu daily"
    );
    await expect(homePage.grid_Load.last()).toBeVisible();
    //Select assessment method
    await homePage.selectAssestMethod_SecondGrid("Assessed");
    //Enter the price value
    await homePage.enterPrice_secondPriceGrid(randomNumber2.toString());
    const lngDESValue = await homePage.priceField.first().textContent();
    const referencePrice = Number(randomNumber1) + Number(randomNumber2);
    console.log("LNG DES Value: " + lngDESValue);
    console.log("LNG DES Argentina Reference Price: " + referencePrice);
    //verify the calculation of reference price
    await expect(homePage.priceField.first()).toContainText(referencePrice.toString());
  });
});
