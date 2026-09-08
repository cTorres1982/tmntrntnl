import { connectToTestDatabase } from "./db-client";
import { E2E_JOB_TITLE_PREFIX } from "./test-constants";

export default async function globalTeardown(): Promise<void> {
  const client = await connectToTestDatabase();

  try {
    await client.query(`DELETE FROM jobs.jobs WHERE title LIKE $1`, [`${E2E_JOB_TITLE_PREFIX}%`]);
  } finally {
    await client.end();
  }
}
