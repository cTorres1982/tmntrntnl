import { expect, test } from "@playwright/test";
import { connectToTestDatabase } from "./db-client";
import { JobsPage } from "./pages/jobs-page";
import { E2E_CUSTOMER_ID, E2E_JOB_TITLE_PREFIX } from "./test-constants";

test.describe("Jobs — create, filter, complete", () => {
  test("creates a job, finds it via a status filter, completes it, and sees the status update", async ({ page }) => {
    const jobTitle = `${E2E_JOB_TITLE_PREFIX} ${Date.now()}`;
    const jobsPage = new JobsPage(page);

    // 1. Navigate to /jobs
    await jobsPage.goto();

    // 2. Create a new job using the modal
    await jobsPage.openCreateJobModal();
    await jobsPage.fillCreateJobForm({
      title: jobTitle,
      description: "Created by the Playwright e2e suite",
      street: "456 Oak St",
      city: "Austin",
      state: "TX",
      zipCode: "78702",
      latitude: "30.30",
      longitude: "-97.80",
      customerId: E2E_CUSTOMER_ID,
    });
    await jobsPage.submitCreateJobForm();

    // 3. Verify the job appears in the table
    await jobsPage.waitForJobRow(jobTitle);
    await expect(jobsPage.statusBadgeFor(jobTitle)).toHaveText("Draft");

    // 4. Filter by status — a freshly created job (no scheduledDate) is Draft
    await jobsPage.toggleStatusFilter("Draft");
    await jobsPage.waitForJobRow(jobTitle);
    await jobsPage.toggleStatusFilter("Draft"); // clear it again before continuing

    // The UI's create-job/filter-jobs/complete-job feature set (AC.md Part 2)
    // has no Schedule/Start action, so a job can never reach InProgress
    // through the UI alone — advancing it here mirrors how the backend's own
    // M6 smoke test reached the same state, and keeps the "complete" step
    // exercising the real Complete button/Server Action rather than a state
    // the UI has no way to produce.
    const client = await connectToTestDatabase();
    try {
      await client.query(
        `UPDATE jobs.jobs SET status = 'InProgress', started_at_utc = now() WHERE title = $1`,
        [jobTitle],
      );
    } finally {
      await client.end();
    }
    await page.reload();
    await jobsPage.waitForJobRow(jobTitle);

    // 5. Complete the job
    await jobsPage.openCompleteModalFor(jobTitle);
    await jobsPage.fillSignatureUrl("https://example.com/signature.png");
    await jobsPage.submitCompleteJobForm();

    // 6. Verify the status changes to "Completed"
    await expect(jobsPage.statusBadgeFor(jobTitle)).toHaveText("Completed");
  });
});
