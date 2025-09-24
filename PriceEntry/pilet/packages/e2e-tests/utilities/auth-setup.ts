import { test as setup, expect, Page } from '@playwright/test';
import { LoginPage } from "../pages/loginPage";
import credentials from "../constants/credentials";
import { HomePage } from "../pages/homePage";
import { Helpers } from "../utilities/helper";
const helper = new Helpers();
const today: Date = new Date();
const cd = today.getDate();
const month = helper.getMonth() || `${today.getMonth() + 1}`;

setup('Login to the Price entry', async ({ page }) => {
    const pageName = "Price entry E2E-Automation" + "-" + cd + "-" + month;
    const pageDescription = "Price entry Page automation testing";
    const loginPage = new LoginPage(page);
    const homePage = new HomePage(page);
    await loginPage.login_PriceEntry(credentials.chaUser.username, credentials.chaUser.password);
    await page.context().storageState({ path: './config/loginAuth.json' });
    await homePage.createPage(pageName, pageDescription)
});
