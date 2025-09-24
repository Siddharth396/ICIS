import http from "k6/http";
import { check, group } from "k6";
import { k6Option } from "../utils/k6Options";
import { getLoginCookie } from "../utils/AuthoringLogin";
import { currentEnvironment } from "../environments";
import { GetContentBlock } from "../fixtures/priceEntryGridData";
import { updatePriceDataQuery } from "../fixtures/updatePriceEntryGridData";
import { htmlReport } from "../javascripts/bundle";
// @ts-ignore
import { textSummary } from "../javascripts/index";
import { testInputs, mutationTestInputs } from "../fixtures/testData";

export const options = k6Option;

// GraphQL query as per your payload
const getPriceEntryData = GetContentBlock;

export function setup() {
    // Get authentication cookie using provided credentials
    return getLoginCookie(
        currentEnvironment.username,
        currentEnvironment.password
    );
}

export default function (cookie: any) {
    const params = {
        cookies: { aauth: cookie },
        headers: { "Content-Type": "application/json" },
    };

    // 1. First, update Price Entry Grid Data using mutation
    group("Update Price Entry Grid Data", () => {
        const mutationPayload = {
            operationName: "updatePriceEntryGridData",
            variables: {
                priceItemInput: {
                    operationType: mutationTestInputs.operationType,
                    seriesId: mutationTestInputs.seriesId,
                    seriesItemTypeCode: mutationTestInputs.seriesItemTypeCode,
                    assessedDateTime: mutationTestInputs.assessedDateTime,
                    seriesItem: mutationTestInputs.seriesItem,
                },
            },
            query: updatePriceDataQuery.query,
        };

        const mutationRes = http.post(
            currentEnvironment.apiurl,
            JSON.stringify(mutationPayload),
            params
        );

        check(mutationRes, {
            "mutation status is 200": (r) => r.status === 200,
            "mutation has id": (r) => !!r.json("data.updatePriceEntryGridData.id"),
        });
    });

    // 2. Then, fetch Price Entry Grid Data using query
    group("Fetch Price Entry Grid Data", () => {
        const queryPayload = {
            operationName: "getContentBlock",
            variables: {
                contentBlockId: testInputs.contentBlockId,
                version: testInputs.version,
                assessedDateTime: testInputs.assessedDateTime,
                includeNotStarted: testInputs.includeNotStarted,
                isReviewMode: testInputs.isReviewMode,
            },
            query: getPriceEntryData,
        };

        const queryRes = http.post(
            currentEnvironment.apiurl,
            JSON.stringify(queryPayload),
            params
        );

        check(queryRes, {
            "query status is 200": (r) => r.status === 200,
            "query has data": (r) => !!r.json("data.contentBlock"),
        });
    });
}

// k6 summary handler: generates HTML and text summary reports after the test run
export function handleSummary(data: any) {
    console.log("Preparing the end-of-test summary...");
    return {
        "./html-results/priceEntryGridDataLoadTest.html": htmlReport(data),
        stdout: textSummary(data, { indent: " ", enableColors: true }),
    };
}

