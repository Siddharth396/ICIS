import http, { RefinedResponse, ResponseType } from "k6/http";
import { currentEnvironment } from "../environments";

export function getLoginCookie(username: string, password: string) {
    try {
        const jar = http.cookieJar();

        const diPage = http.get(currentEnvironment.authoringloginUrl, { jar });

        let loginPage: RefinedResponse<ResponseType | undefined>;
        try {
            // Click on "CHA" button
            loginPage = diPage.clickLink({
                selector: "div.col-md-12 > a[href]",
                params: { jar },
            });
        } catch {
            loginPage = diPage;
        }

        // Insert login username and password
        const loginPageResponse = loginPage.submitForm({
            formSelector: "form",
            fields: {
                UserName: username,
                Password: password,
            },
            submitSelector: "#submitButton",
            params: { jar },
        });

        let redirectResponse = loginPageResponse.submitForm({
            params: { jar },
        });

        redirectResponse = redirectResponse.submitForm({ params: { jar } });

        const cookie = jar.cookiesForURL(redirectResponse.url)["aauth"][0];

        if (cookie) {
            console.log(`Successfully logged in as: ${username}`);
        } else {
            console.error(`Login failed for user: ${username}`);
        }

        return cookie;
    } catch (e) {
        console.error(`Could not log on user: ${username}`, e);
        return null;
    }
}

