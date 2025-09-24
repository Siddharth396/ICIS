import { expect, type Locator, type Page } from "@playwright/test";

export class LoginPage {
  readonly page: Page;
  readonly chaLink: Locator;
  readonly userName: Locator;
  readonly password: Locator;
  readonly signInButton: Locator;
  readonly homePage: Locator;
  readonly createNewPageButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.chaLink = page.getByRole("link", { name: "CHA" });
    this.userName = page.locator("#userNameInput");
    this.password = page.locator("#passwordInput");
    this.signInButton = page.getByRole("button", { name: "Sign in" });
    this.homePage = page.locator("div[data-testid='authoring-view']");
    this.createNewPageButton = page.getByTestId("create-new-page-button");
  }
  async goto() {
    await this.page.goto('/content/pricing/',{waitUntil: 'load'});
    await expect(this.createNewPageButton).toHaveAccessibleName("Create New")
  }
  async login_PriceEntry(username: string, password: string){
    await this.page.goto('/content/pricing/',{waitUntil: 'load'});
    await this.chaLink.click()
    await this.userName.fill(username)
    await this.password.fill(password)
    await this.signInButton.click()
    await expect(this.createNewPageButton).toHaveAccessibleName("Create New")
  }
}
