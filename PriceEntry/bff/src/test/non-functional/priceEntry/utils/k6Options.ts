/**
 * Common k6 options for performance, throughput, and availability NFRs:
 *
 * Availability:
 * - Target 99.99% availability across a year's rolling window (measured by ops, not k6, but reflected in error thresholds)
 *
 * Performance:
 * - First Contentful Paint (FCP) under 1s for 75% of user interactions (p(75)<1000)
 * - Largest Contentful Paint (LCP) under 2s for 75% of user interactions (p(75)<2000)
 * - Workflow actions under 1s for 75% (p(75)<1000)
 *
 * Throughput:
 * - Support 10 concurrent users for LNG Entry
 * - Ramp up to 25 users for PA-PROD
 * - Hold at 25 users
 * - Ramp up to 200 users for PA-SCALE (future)
 * - Hold at 200 users
 * - Ramp down to 0 users
 *
 * You can override stages in your test file or via CLI for different scenarios.
 * Example: k6 run dist/period-generator-tests.js
 */
export const k6Option = {
    stages: [
        { duration: "2m", target: 10 },   // Ramp up to 10 users (LNG Entry)
        { duration: "2m", target: 25 },   // Ramp up to 25 users (PA-PROD)
        { duration: "3m", target: 25 },   // Hold at 25 users
        { duration: "3m", target: 200 },  // Ramp up to 200 users (PA-SCALE, future)
        { duration: "5m", target: 200 },  // Hold at 200 users
        { duration: "2m", target: 0 },    // Ramp down to 0 users
    ],
    insecureSkipTLSVerify: false, // Only set to true for local/dev with self-signed certs
    thresholds: {
        // Performance thresholds
        http_req_duration: [
            'avg<1000',    // Average response time < 1s
            'p(75)<1000',  // 75% of requests under 1s (FCP/Workflow)
            'p(90)<2000',  // 90% of requests under 2s
            'p(95)<2000',  // 95% of requests under 2s (LCP)
            'p(99)<3000'   // 99% of requests under 3s (for peak/scale stage)
        ],
        // Availability & error thresholds
        http_req_failed: ['rate<0.0005'], // <0.05% error rate (matches 99.95%+ availability)
        checks: ['rate>0.99'],            // 99% of checks should pass
    },
};