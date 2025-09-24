type Env = "performance"; // Only allow 'performance' environment

const environments: Record<
    Env,
    {
        authoringloginUrl: string;
        apiurl: string;
        username: string;
        password: string;
    }
> = {
    performance: {
        authoringloginUrl:
            "https://authoring.performance.cha.rbxd.ds/content/pricing/",
        apiurl: "https://authoring.performance.cha.rbxd.ds/api/price-entry/v1/graphql",
        username: __ENV.PRICE_ENTRY_TEST_USER,
        password: __ENV.PRICE_ENTRY_TEST_PWD,
    },
};

// Only allow running in 'performance' environment to prevent accidental runs on other environments
const environment: Env = (__ENV.ENVIRONMENT as Env) || "performance";
if (environment !== "performance") {
    throw new Error("Load tests are restricted to the 'performance' environment only.");
}

export const loadTestConfig = {
    executor: "constant-arrival-rate",
    rate: 1,
    timeUnit: "1s",
    duration: "10s",
    gracefulStop: "5s",
    preAllocatedVUs: 1,
    maxVUs: 5,
};
export const httpreqdurationMetric = ["p(90) < 6000", "med < 2500"];
export const httpfailedMetric = ["rate == 0"];
export const checkMetric = ["rate == 1"];

export const currentEnvironment = environments[environment];
export const currentEnvValue = environment;
export default environments;
