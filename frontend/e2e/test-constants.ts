/** Must match .env.local's DEV_ORGANIZATION_ID — the tenant the Api/Server Actions/Route Handler send as X-Organization-Id. */
export const E2E_ORGANIZATION_ID = "11111111-1111-1111-1111-111111111111";

/** A fixed, well-known customer row seeded by global-setup.ts so create-job's FK constraint has something valid to point at. */
export const E2E_CUSTOMER_ID = "99999999-9999-9999-9999-999999999999";

/** Distinguishes E2E-created rows from anything else in the database, for teardown and for avoiding cross-run collisions. */
export const E2E_JOB_TITLE_PREFIX = "E2E Test Job";
