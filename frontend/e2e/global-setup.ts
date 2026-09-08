import { connectToTestDatabase } from "./db-client";
import { E2E_CUSTOMER_ID, E2E_JOB_TITLE_PREFIX, E2E_ORGANIZATION_ID } from "./test-constants";

/**
 * Seeds the one row create-job's FK constraint needs (jobs.customers) — the
 * backend has no API surface for creating customers (Customer is an
 * out-of-scope placeholder, see DataBase/schema.sql), so the only way to get
 * a valid customerId is a direct insert. Also clears any leftover rows from a
 * previous failed run so tests don't see stale E2E data.
 */
export default async function globalSetup(): Promise<void> {
  const client = await connectToTestDatabase();

  try {
    await client.query(
      `INSERT INTO jobs.customers (id, organization_id, name)
       VALUES ($1, $2, 'E2E Test Customer')
       ON CONFLICT (id) DO NOTHING`,
      [E2E_CUSTOMER_ID, E2E_ORGANIZATION_ID],
    );

    await client.query(`DELETE FROM jobs.jobs WHERE title LIKE $1`, [`${E2E_JOB_TITLE_PREFIX}%`]);
  } finally {
    await client.end();
  }
}
