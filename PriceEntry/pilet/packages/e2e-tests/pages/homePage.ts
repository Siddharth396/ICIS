/**
 * Represents the HomePage class which contains methods and locators for interacting with the home page of the application.
 */
import { type Locator, type Page, expect } from "@playwright/test";
import moment from "moment";
const testData = JSON.parse(JSON.stringify(require("../test-data/TestData.json")));
import { Helpers } from "../utilities/helper";
const helper = new Helpers();
const previousDay = helper.getPreviousDate();
const month = helper.getMonth();
let pageId: string;

export class HomePage {
  readonly page: Page;
  readonly commodityLabel: Locator;
  readonly datePicker: Locator;
  readonly commodity_Dropdown_Indicator: Locator;
  readonly region_Dropdown_Indicator: Locator;
  readonly priceCategory_Dropdown_Indicator: Locator;
  readonly transactionType_Dropdown_Indicator: Locator;
  readonly assessedFrequency_Dropdown_Indicator: Locator;
  readonly priceSeries_Dropdown_Menu: Locator;
  readonly commodity_Dropdown_List: Locator;
  readonly commodity_Dropdown_List_Items: Locator;
  readonly commodity_Item: Locator;
  readonly header_Titles: Locator;
  readonly assesMethod_Field: Locator;
  readonly priceField: Locator;
  readonly priceField_CSS: Locator;
  readonly referenceField: Locator;
  readonly premiumDiscountField: Locator;
  readonly priceField_Input: Locator;
  readonly dataUsed_Field: Locator;
  readonly calendar: Locator;
  readonly lastAssesmentPriceField: Locator;
  readonly priceChange: Locator;
  readonly lowPrice: Locator;
  readonly lowPrice_Input: Locator;
  readonly highPrice_Input: Locator;
  readonly midPrice: Locator;
  readonly highPrice: Locator;
  readonly toolTip_ErrorMessage: Locator;
  readonly editButtonWrapper: Locator;
  readonly editButton: Locator;
  readonly checkbox_Melamine_modal_Popup: Locator;
  readonly saveButton_modal_Popup: Locator;
  readonly grid_Load: Locator;
  readonly statusText: Locator;
  readonly priceSeries_PopupWindow: Locator;
  readonly select_All_PriceSeries: Locator;
  readonly vertical_Scrollbar_modalPopup: Locator;
  readonly commodity_Dropdown_List_Box: Locator;
  readonly singlePriceValue: Locator;
  readonly singlePriceValue_Title_Grid: Locator;
  readonly click_Price_Series_Title: Locator;
  readonly priceCommentary_Heading: Locator;
  readonly priceCommentary_Editor: Locator;
  readonly text_Box_CrossButton: Locator;
  readonly showHide_Column_Tab: Locator;
  readonly showHide_Column_Dropdown: Locator;
  readonly createNewPageButton: Locator;
  readonly createNewPagePopupWindow: Locator;
  readonly pageHeadLineName: Locator;
  readonly pagedescription: Locator;
  readonly createPage_nextButton: Locator;
  readonly selectTemplate: Locator;
  readonly createPageButton: Locator;
  readonly addContentBlock: Locator;
  readonly pageTitle: Locator;
  readonly priceEntryContentBlock: Locator;
  readonly priceGrid: Locator;
  readonly deletePriceGrid_HomePage: Locator;
  readonly deletePriceGrid_Popup: Locator;
  readonly deletePriceGrid_DeleteButton: Locator;
  readonly selectPriceSeries_Button: Locator;
  readonly priceEntryContentBlockTitle: Locator;
  readonly workflowButton: Locator;
  readonly workflowModalPopup: Locator;
  readonly publishButton_modalPopup: Locator;
  readonly closePopupModalWindow: Locator;
  readonly reviewRequestLink: Locator;
  readonly referencePriceDropdown: Locator;
  readonly referencePriceDropdownValues: Locator;
  readonly calendar_Month: Locator;
  readonly calendar_Year: Locator;
  readonly calendar_PreviousMonth_Navigation: Locator;
  readonly calendar_NextMonth_Navigation: Locator;
  readonly calendar_Day: Locator;
  readonly calendar_currentYearText: Locator;
  readonly calendar_Year_Dropdown: Locator;
  readonly premiumDiscountInputField: Locator;
  readonly priceEntryGridTitle: Locator;

  constructor(page) {
    this.page = page;
    this.commodityLabel = page.locator("#content");
    this.datePicker = page.locator("div[data-testid='price-entry-date-picker']");
    this.commodity_Dropdown_Indicator = page.getByTestId("commodity-select");
    this.region_Dropdown_Indicator = page.getByTestId("region-select");
    this.priceCategory_Dropdown_Indicator = page.getByTestId("price-category-select");
    this.transactionType_Dropdown_Indicator = page.getByTestId("transaction-type-select");
    this.assessedFrequency_Dropdown_Indicator = page.getByTestId("frequency-select");
    this.priceSeries_Dropdown_Menu = page.locator("div[class*='select__menu-list']");
    this.commodity_Dropdown_List = page.locator("#react-select-2-listbox");
    this.commodity_Dropdown_List_Items = page.locator(".select__menu-list div");
    this.commodity_Item = page.locator("div[class*='select__pi-single']");
    this.header_Titles = page.locator(".ag-header-cell-comp-wrapper div");
    this.assesMethod_Field = page.locator(
      "div[role='rowgroup'] .ag-row-first div[col-id='assessmentMethod']"
    );
    this.priceField = page.getByTestId('text-box');
    this.priceField_CSS = page.locator("div[role='rowgroup'] .ag-row-first div[col-id='price']");
    this.referenceField = page.locator(
      "div[role='rowgroup'] .ag-row-first div[col-id='referencePrice']"
    );
    this.premiumDiscountField = page.locator(
      "div[role='rowgroup'] .ag-row-first div[col-id='premiumDiscount']"
    );
    this.priceField_Input = page.getByTestId('text-box');
    this.lowPrice_Input = page.locator(
      "div[role='rowgroup'] .ag-row-first div[col-id='priceLow'] div input"
    );
    this.highPrice_Input = page.locator(
      "div[role='rowgroup'] .ag-row-first  div[col-id='priceHigh'] div input"
    );
    this.dataUsed_Field = page.locator("div[role='rowgroup'] .ag-row-first div[col-id='dataUsed']");
    this.calendar = page.getByTestId("date-selector");
    this.lastAssesmentPriceField = page.locator(
      "div[role='rowgroup'] .ag-row-first div[col-id='lastAssessmentPrice']"
    );
    this.priceChange = page.locator("span[data-testid='movement']");
    this.lowPrice = page.locator("div[col-id='priceLow'] div[type='rightAligned']");
    this.midPrice = page.locator("div[role='rowgroup'] .ag-row-first div[col-id='priceMid']");
    this.highPrice = page.locator("div[col-id='priceHigh'] div[type='rightAligned']");
    this.toolTip_ErrorMessage = page.locator("div[role='dialog'] div p");
    this.editButtonWrapper = page.locator(
      "div[class*='ContentBlockWrapperstyle__EditButtonWrapper-sc-']"
    );
    this.editButton = page.locator(
      "button[data-testid*='price-entry-content-block-wrapper-edit-button']"
    );
    this.checkbox_Melamine_modal_Popup = page
      .locator(".ag-selection-checkbox div[class*='ag-checkbox ag-input-field']")
      .first();
    //this.saveButton_modal_Popup = page.locator("button[data-testid='price-entry-modal-save-button']")
    this.saveButton_modal_Popup = page.getByRole("button", { name: "Save" });
    this.grid_Load = page.locator(
      "div[class='ag-root-wrapper-body ag-focus-managed ag-layout-auto-height']"
    );
    this.statusText = page.getByTestId("price-series-item-status");
    this.priceSeries_PopupWindow = page.getByTestId("price-entry-series-select-modal");
    this.select_All_PriceSeries = page.locator(
      "div[role='columnheader'][col-id='checkbox'] div[class*='ag-checkbox-input-wrapper']"
    );
    this.vertical_Scrollbar_modalPopup = page.locator(".ag-body-vertical-scroll-viewport");
    this.commodity_Dropdown_List_Box = page.locator("div[id*='listbox']");
    this.singlePriceValue = page.locator("div[class*='cell-0-name']").first();
    this.singlePriceValue_Title_Grid = page.locator(
      ".cell-0-priceSeriesName span[id*='cell-priceSeriesName']"
    );
    this.click_Price_Series_Title = page.locator(".ag-header-row.ag-header-row-column div[col-id='priceSeriesName']");
    this.priceCommentary_Heading = page.locator("h4[class*='Headingstyle']");
    this.priceCommentary_Editor = page
      .getByTestId("price-entry-content-block-wrapper")
      .getByRole("paragraph");
    this.text_Box_CrossButton = page.getByTestId("text-close-button");
    this.showHide_Column_Tab = page.getByTestId("edit-column-button");
    this.showHide_Column_Dropdown = page.getByTestId("edit-columns-wrapper");
    this.createNewPageButton = page.getByTestId("create-new-page-button");
    this.createNewPagePopupWindow = page.getByTestId("new-page-modal");
    this.pageHeadLineName = page.getByTestId("headline-textarea");
    this.pagedescription = page.getByTestId("description-textarea");
    this.createPage_nextButton = page.getByTestId("next-button");
    this.selectTemplate = page.getByTestId("template-image-wrapper-One Column");
    this.createPageButton = page.getByTestId("create-page-button");
    this.addContentBlock = page.locator('svg[aria-hidden="true"][data-icon="plus"]');
    this.pageTitle = page.getByTestId("page-title");
    this.priceEntryContentBlock = page.getByTestId("content-block-button-price-entry-capability");
    this.priceGrid = page.locator("div[data-testid*='locked-content-overlay-hidden']");
    this.deletePriceGrid_HomePage = page.locator("button[title='Delete']");
    this.deletePriceGrid_Popup = page.getByTestId("delete-content-block-modal");
    this.deletePriceGrid_DeleteButton = page.getByTestId("modal-footer-delete-btn");
    this.selectPriceSeries_Button = page.getByRole("button", { name: "Select prices" });
    this.priceEntryContentBlockTitle = page.getByRole('textbox', { name: 'title-input' });
    this.workflowButton = page.getByTestId("workflow-button");
    this.workflowModalPopup = page.getByTestId("workflow-validation-modal");
    this.publishButton_modalPopup = page.getByTestId("modal-footer-publish-btn");
    this.closePopupModalWindow = page.getByTestId("dismiss-modal");
    this.reviewRequestLink = page.getByTestId("workspace-nav-item-review-requests");
    this.referencePriceDropdown = page
      .getByTestId("price-entry-grid-container")
      .locator("svg")
      .nth(1);
    this.referencePriceDropdownValues = page.locator("div[role='listbox'] div");
    this.calendar_Month = page.locator("span[data-testid='month-selection-value']");
    this.calendar_Year = page.locator("span[data-testid='year-selection-value']");
    this.calendar_PreviousMonth_Navigation = this.page.locator("span[data-testid='month-navigation-previous']");
    this.calendar_NextMonth_Navigation = this.page.locator("span[data-testid='month-navigation-next']");
    this.calendar_currentYearText = page.locator("span[data-testid='year-selection-value']");
    this.calendar_Year_Dropdown = page.locator("div[data-testid='year-selection-dropdown']");
    this.premiumDiscountInputField = page.getByTestId("text-box")
    this.priceEntryGridTitle = page.getByPlaceholder("Enter grid title");
  }

  async verify_Commodity_List() {
    const items = this.commodity_Dropdown_List_Items;
    for (let i = 0; i < (await items.count()); i++) {
      await expect(items.nth(i)).toHaveText(testData.Commodity_Items[i]);
    }
  }
  async selectCommodity(comm: string) {
    await this.commodity_Dropdown_Indicator.click();
    await this.commodity_Dropdown_List_Box.getByText(comm, { exact: true }).click();
  }
  async selectAssestMethod(method: string) {
    await this.assesMethod_Field.last().focus();
    await this.assesMethod_Field.last().click();
    await this.page.getByRole("option", { exact: true }).getByText(method).click();
    await this.page.waitForTimeout(500);
  }
  async selectAssestMethod_SecondGrid(method: string) {
    await this.page.locator("(//div[@role='rowgroup']//div[contains(@class,'ag-row-first')]/div[@col-id='assessmentMethod'])[2]").click();
    await this.page.getByRole("option", { exact: true }).getByText(method).click();
    await this.page.waitForTimeout(500);
  }
  async selectReferencePriceDropdownValue(value: string) {
    await this.referencePriceDropdown.click();
    await this.page.getByRole('option', { name: 'LNG DES Argentina' }).click();
  }
  async selectDataUsed(method: string) {
    await this.dataUsed_Field.click();
    await this.page.getByRole("option", { name: method }).click();
  }
  async enterPrice(price: string) {
    await this.priceField.first().click();
    await this.priceField_Input.fill(price);
    await this.priceField_Input.press("Enter");
    await this.page.waitForTimeout(1000);
  }
  async enterPremiumDiscountValue(price: string) {
    await this.premiumDiscountField.click();
    await this.premiumDiscountInputField.fill(price);
    await this.premiumDiscountInputField.press("Enter");
    await this.page.waitForTimeout(1000);
  }
  async enterPrice_secondPriceGrid(price: string) {
    await this.priceField.last().click();
    await this.priceField_Input.fill(price);
    await this.priceField_Input.press("Enter");
    await this.page.waitForTimeout(2000);
  }
  async selectDate(date: string) {
    await this.calendar.click();
    await this.page.getByText(date, { exact: true }).click();
  }
  async enterLowPrice(price: string) {
    await this.lowPrice.click();
    await this.lowPrice_Input.fill(price);
    await this.lowPrice_Input.press("Enter");
    await this.page.waitForTimeout(1000);
  }
  async enterHighPrice(price: string) {
    await this.highPrice.click();
    await this.highPrice_Input.fill(price);
    await this.highPrice_Input.press("Enter");
    await this.page.waitForTimeout(1000);
  }
  async clickEditButton() {
    await this.editButtonWrapper.hover();
    await this.editButton.click();
    await expect(this.priceSeries_PopupWindow).toBeVisible();
    await expect(
      this.page.getByTestId("price-entry-series-select-modal").getByRole("heading")
    ).toContainText("Select price series");
  }
  async clearLowPriceField() {
    //clear the low price field
    await this.lowPrice.click();
    if ((await this.lowPrice_Input.inputValue()).length > 0) {
      await this.lowPrice.click();
      await this.text_Box_CrossButton.click();
      await this.lowPrice.press("Enter");
      expect(this.lowPrice).toBeEmpty;
    } else {
      console.log("Low price field is empty");
      await this.lowPrice.press("Enter");
    }
  }
  async clearHighPriceField() {
    await this.highPrice.click();
    if ((await this.highPrice_Input.inputValue()).length > 0) {
      await this.highPrice.click();
      await this.text_Box_CrossButton.click();
      await this.highPrice.press("Enter");
      expect(this.highPrice).toBeEmpty;
    } else {
      console.log("High price field is empty");
      await this.highPrice.press("Enter");
    }
  }
  async clearPriceInputField() {
    await this.priceField.click();
    if ((await this.priceField_Input.inputValue()).length > 0) {
      await this.priceField.click();
      await this.text_Box_CrossButton.click();
      await this.click_Price_Series_Title.click();
      await this.page.waitForTimeout(500);
      expect(this.priceField).toBeEmpty;
    } else {
      console.log("Price field is empty");
      await this.click_Price_Series_Title.click();
    }
  }
  async selectDate_calendar(date: number, monthToSelect: string) {
    await this.calendar.click();
    const mm = this.page.locator("span[data-testid='month-selection-value']");
    const yy = this.page.locator("span[data-testid='year-selection-value']");
    const prev = this.page.locator("span[data-testid='month-navigation-previous']");
    const next = this.page.locator("span[data-testid='month-navigation-next']");
    let currentMonth = moment(await mm.textContent(), "MMMM");
    const targetMonth = moment(monthToSelect, "MMMM");

    while (currentMonth.format("MMMM") !== targetMonth.format("MMMM")) {
      if (targetMonth.isBefore(currentMonth)) {
        await prev.click();
      } else {
        await next.click();
      }
      currentMonth = moment(await mm.textContent(), "MMMM");
    }
    await this.page.click(
      `//button[@class='rdp-button_reset rdp-button rdp-day' or  @class='rdp-button_reset rdp-button rdp-day rdp-day_today' or @class='rdp-button_reset rdp-button rdp-day rdp-day_selected' or @class='rdp-button_reset rdp-button rdp-day rdp-day_selected rdp-day_today'][text()='${date}']`
    );
  }
  async selectPriceSeries(comm: string) {
    //Select Commodity from the commodity dropdown
    await this.commodity_Dropdown_Indicator.click();
    await this.commodity_Dropdown_List_Box.getByText(comm, { exact: true }).click();
    await this.select_All_PriceSeries.click();
    await this.saveButton_modal_Popup.click();
    await expect(this.grid_Load).toBeVisible();
  }
  async selectSinglePriceSeries(
    comm: string,
    region: string,
    priceCategory: string,
    transactionType: string,
    assessedFrequency: string,
    priceSeries: string
  ) {
    //Select filters from the dropdowns
    await this.commodity_Dropdown_Indicator.click();
    await this.commodity_Dropdown_List_Box.getByText(comm, { exact: true }).click();
    await this.region_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(region, { exact: true }).click();
    await this.priceCategory_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(priceCategory, { exact: true }).click();
    await this.transactionType_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(transactionType, { exact: true }).click();
    await this.assessedFrequency_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(assessedFrequency, { exact: true }).click();
    await this.page
      .locator("div[class='ag-body ag-layout-normal'] div[class='ag-full-width-container']")
      .first()
      .focus();
    await this.scrollUntilElementIsVisible(priceSeries);
    await this.saveButton_modal_Popup.click();
  }
  async selectSinglePriceSeries_Styrene(
    comm: string,
    region: string,
    priceCategory: string,
    transactionType: string,
    assessedFrequency: string,
    priceSeries: string
  ) {
    //Select filters from the dropdowns
    await this.commodity_Dropdown_Indicator.click();
    await this.commodity_Dropdown_List_Box.getByText(comm, { exact: true }).click();
    await this.region_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(region, { exact: true }).click();
    await this.priceCategory_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(priceCategory, { exact: true }).click();
    await this.transactionType_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(transactionType, { exact: true }).click();
    await this.assessedFrequency_Dropdown_Indicator.click();
    await this.priceSeries_Dropdown_Menu.getByText(assessedFrequency, { exact: true }).click();
    await this.commodity_Dropdown_List_Box.getByText(comm, { exact: true }).click();
    await this.page
      .locator("div[class='ag-body ag-layout-normal'] div[class='ag-full-width-container']")
      .last()
      .focus();
    await this.scrollUntilElementIsVisible_Styrene(priceSeries);
    await this.saveButton_modal_Popup.click();
    await expect(this.grid_Load).toBeVisible();
  }
  async scrollUntilElementIsVisible(priceSeries: string) {
    const priceSeriesElement = this.page.getByRole("row", { name: priceSeries }).last();
    const scrollBarExists = await this.vertical_Scrollbar_modalPopup.count() > 0;

    while (!(await priceSeriesElement.isVisible())) {
      if (scrollBarExists) {
        await this.vertical_Scrollbar_modalPopup.last().click();
        await this.page.mouse.wheel(0, 500);
        await this.page.waitForTimeout(100);
      } else {
        break;
      }
    }

    if (await priceSeriesElement.isVisible()) {
      await priceSeriesElement.click();
    } else {
      throw new Error(`Price series "${priceSeries}" is not visible in the modal popup.`);
    }

  }
  async scrollUntilElementIsVisible_Styrene(priceSeries: string) {
    while (!(await this.page.getByRole("row", { name: priceSeries }).last().isVisible())) {
      await this.vertical_Scrollbar_modalPopup.last().click();
    }
    await this.page.getByRole("row", { name: priceSeries }).last().click();
  }
  async selectOnlyCommadity(comm: string) {
    //Select Commodity from the commodity dropdown
    await this.commodity_Dropdown_Indicator.click();
    await this.commodity_Dropdown_List_Box.getByText(comm, { exact: true }).click();
  }
  async enterPriceCommentary(commentary: string) {
    await this.priceCommentary_Editor.first().click();
    await this.priceCommentary_Editor.first().fill(commentary);
    await this.click_Price_Series_Title.click();
  }
  async getDate_Fromcalendar() {
    return await this.page.locator("div[data-testid='price-entry-date-picker'] span").textContent();
  }
  async click_NonManadatoryColumn(columnName: string) {
    await this.showHide_Column_Tab.click();
    await this.page.getByTestId("edit-columns-wrapper").getByText(columnName).click();
    await this.showHide_Column_Tab.click();
  }
  async click_NonManadatoryColumns_Multiple(columnNames: Array<string>) {
    await this.showHide_Column_Tab.click();
    for (const columnName of columnNames) {
      await this.page.getByTestId("edit-columns-wrapper").getByText(columnName).click();
      await this.page.waitForTimeout(1000);
    }
    await this.showHide_Column_Tab.click();
  }
  async createPage(pageName: string, pageDescription: string) {
    await this.createNewPageButton.click();
    await expect(this.createNewPagePopupWindow).toBeVisible();
    await this.pageHeadLineName.fill(pageName);
    await this.pagedescription.fill(pageDescription);
    await expect(this.createPage_nextButton).toHaveRole("button");
    await this.createPage_nextButton.click();
    await expect(this.page.getByRole("heading", { name: "Create new page" })).toBeVisible();
    await this.selectTemplate.click();
    await expect(this.createPageButton).toHaveRole("button");
    await this.createPageButton.click();
    await expect(this.page.getByTestId("page-title")).toContainText(pageName);
  }
  async checkForthePriceGridAvailablity() {
    const gridTitle = "automation";
    if ((await this.selectPriceSeries_Button.count()) > 0) {
      await this.selectPriceSeries_Button.click();
    } else {
      await this.addContentBlock.first().click();
      await this.priceEntryContentBlock.click();
      await expect(this.selectPriceSeries_Button).toBeVisible();
      await this.enterContentBlockTitle(gridTitle);
      const gridTitle1 = this.page.locator("input[aria-label='title-input']");
      await expect(gridTitle1).toHaveValue(gridTitle);
      await this.selectPriceSeries_Button.click();
    }
  }
  async addPriceEntryCapability() {
    const gridTitle = "automationTest";
    await this.addContentBlock.last().click();
    await this.priceEntryContentBlock.click();
    await expect(this.selectPriceSeries_Button).toBeVisible();
    await this.enterContentBlockTitle(gridTitle);
    const gridTitle1 = this.page.locator("input[aria-label='title-input']").last();
    await expect(gridTitle1).toHaveValue(gridTitle);
    await this.selectPriceSeries_Button.click();
  }
  async deletePriceGrid() {
    //delete the price grid(s)
    const priceGridCount = await this.priceGrid.count();
    for (let i = 0; i < priceGridCount; i++) {
      await this.priceGrid.first().hover();
      await this.deletePriceGrid_HomePage.first().click();
      await expect(this.deletePriceGrid_Popup).toBeVisible();
      await this.deletePriceGrid_DeleteButton.click();
    }
    await expect(this.priceGrid).not.toBeVisible();
  }
  async addLastAssessmentPriceForPreviousDate(
    comm: string,
    region: string,
    priceCategory: string,
    transactionType: string,
    assessedFrequency: string,
    priceSeries: string,
    method: string
  ) {
    let [randomNumber] = helper.getRandomNumber();
    //select commodity
    await this.selectSinglePriceSeries(
      comm,
      region,
      priceCategory,
      transactionType,
      assessedFrequency,
      priceSeries
    );
    //Select Previous date
    await this.selectDate_calendar(previousDay, month);
    //Click Assessment method dropdown & Select field values
    await this.selectAssestMethod(method);
    await this.enterPrice(randomNumber.toString());
    await expect(this.priceField.first()).toContainText(randomNumber.toString());
  }
  async selectPreviousYearAndDate(date: number, monthToSelect: string) {
    await this.calendar.click();
    const calendar_currentYearText = await this.page
      .locator("span[data-testid='year-selection-value']")
      .textContent();
    // Click the year dropdown to open it
    await this.calendar_Year_Dropdown.click();
    // Get the current year from the dropdown
    if (calendar_currentYearText === null) {
      throw new Error("Failed to retrieve the current year text.");
    }
    const currentYear = parseInt(calendar_currentYearText.trim());
    // Calculate the previous year
    const previousYear = currentYear - 3;
    // Select the previous year from the dropdown
    await this.page.getByText(previousYear.toString(), { exact: true }).click();
    let currentMonth = moment(await this.calendar_Month.textContent(), "MMMM");
    const targetMonth = moment(monthToSelect, "MMMM");
    // Select the target month
    while (currentMonth.format("MMMM") !== targetMonth.format("MMMM")) {
      if (targetMonth.isBefore(currentMonth)) {
        await this.calendar_PreviousMonth_Navigation.click();
      } else {
        await this.calendar_NextMonth_Navigation.click();
      }
      currentMonth = moment(await this.calendar_Month.textContent(), "MMMM");
    }
    // Select only Sunday dates
    const days = await this.page
      .locator(
        `//button[@class='rdp-button_reset rdp-button rdp-day' or @class='rdp-button_reset rdp-button rdp-day rdp-day_today' or @class='rdp-button_reset rdp-button rdp-day rdp-day_selected' or @class='rdp-button_reset rdp-button rdp-day rdp-day_selected rdp-day_today']`
      )
      .allTextContents();
    for (const day of days) {
      const dayNumber = parseInt(day.trim());
      const dayMoment = moment(
        `${previousYear}-${targetMonth.format("MM")}-${dayNumber}`,
        "YYYY-MM-DD"
      );
      if (dayMoment.day() === 0 && dayMoment.isBefore(moment(), "day")) {
        // Check if the day is Sunday and not the current date
        await this.page.click(
          `//button[@class='rdp-button_reset rdp-button rdp-day' or @class='rdp-button_reset rdp-button rdp-day rdp-day_today' or @class='rdp-button_reset rdp-button rdp-day rdp-day_selected' or @class='rdp-button_reset rdp-button rdp-day rdp-day_selected rdp-day_today'][text()='${dayNumber}']`
        );
        break;
      }
    }
  }
  async enterContentBlockTitle(title: string) {
    await this.priceEntryContentBlockTitle.last().click();
    await this.priceEntryContentBlockTitle.last().fill(title);
    await this.page.getByTestId("save-status").click();
  }
  async enterGridTitle(title: string) {
    await this.priceEntryGridTitle.last().click();
    await this.priceEntryGridTitle.last().fill(title);
    await this.page.getByTestId("save-status").click();
  }
  async verifySendForReviewPopup_withoutdata() {
    await expect(this.page.getByRole("heading", { name: "Send for review" })).toBeVisible();
    await expect(this.workflowModalPopup).toContainText(
      "You are about to send the prices for review.Are you sure you would like to proceed?"
    );
    await expect(this.publishButton_modalPopup).toBeVisible();
    await this.closePopupModalWindow.click();
  }
  async verifySendForReviewWarningPopup_withoutdata() {
    await this.publishButton_modalPopup.click();
    await expect(
      this.page.getByRole("heading", { name: "Prices not sent for review" })
    ).toBeVisible();
    await expect(this.workflowModalPopup).toContainText(
      "Please complete all prices before they can be sent for review"
    );
    await this.closePopupModalWindow.click();
  }
  async workFlow_ReviewPage() {
    const context = this.page.context();
    const page1 = await context.newPage();
    await page1.goto("https://authoring.systest.cha.rbxd.ds/review-requests");
    await page1.getByTestId("page-status-filter-READY_FOR_REVIEW").click();
    await page1.getByRole("row", { name: "automation" }).getByTestId("review-btn").click();
    await expect(this.grid_Load).toBeVisible();
    await expect(page1.getByTestId("price-entry-content-block-wrapper")).toContainText(
      "Release review",
      { timeout: 10000 }
    );
    await expect(page1.getByTestId("price-entry-content-block-wrapper")).toContainText("Send back");
    await expect(page1.getByTestId("price-entry-content-block-wrapper")).toContainText("Approve");
    await expect(page1.getByTestId("edit-column-button")).toBeVisible();
    await expect(this.statusText).toContainText("IN REVIEW");
    await page1.getByRole("button", { name: "Approve" }).click();
    await expect(page1.getByTestId("workflow-validation-modal")).toContainText(
      "Approve submission"
    );
    await expect(page1.getByTestId("workflow-validation-modal")).toContainText(
      "You are about to approve the submission which will publish prices to our subscribers across ICIS Clarity, ICIS API.Are you sure you would like to approve?"
    );
    await expect(page1.getByTestId("modal-footer-publish-btn")).toContainText("Approve");
    await page1.getByTestId("modal-footer-publish-btn").click();
    await expect(page1.getByTestId("workflow-validation-modal")).toContainText(
      "Submission approved"
    );
    await page1.getByTestId("dismiss-modal").click();
    await page1.close();
  }
  async getPageId() {
    const url = this.page.url();
    const pageId = url.split("/").pop();
    return pageId;
  }
  async waitForPageLink(workspace: string, pageId: string) {
    const url = `/${workspace}/${pageId}`;
    return await this.page.waitForSelector(`a[data-testid="page-title-link"][href="${url}"]`, { timeout: 5000 });
  }
  async fetchPageId() {
    const pageId = await this.getPageId();
    return pageId;
  }
}
