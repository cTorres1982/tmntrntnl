import { expect, type Locator, type Page } from "@playwright/test";
import type { CreateJobFormData } from "./create-job-form-data.type";

/** Page Object Model for /jobs (AC.md 5.3's explicit requirement) — every locator is data-testid-based, never text/CSS, so UI copy changes don't break the suite. */
export class JobsPage {
  readonly page: Page;
  readonly createJobButton: Locator;
  readonly createJobModal: Locator;
  readonly completeJobModal: Locator;
  readonly jobsTable: Locator;
  readonly filterBar: Locator;

  constructor(page: Page) {
    this.page = page;
    this.createJobButton = page.getByTestId("open-create-job-button");
    this.createJobModal = page.getByTestId("create-job-modal");
    this.completeJobModal = page.getByTestId("complete-job-modal");
    this.jobsTable = page.getByTestId("jobs-table");
    this.filterBar = page.getByTestId("job-filter-bar");
  }

  async goto(): Promise<void> {
    await this.page.goto("/jobs");
    await this.page.waitForSelector('[data-testid="jobs-table"], [data-testid="jobs-empty-state"]');
  }

  async openCreateJobModal(): Promise<void> {
    await this.createJobButton.click();
    await expect(this.createJobModal).toBeVisible();
  }

  async fillCreateJobForm(data: CreateJobFormData): Promise<void> {
    const modal = this.createJobModal;
    await modal.getByLabel("Title").fill(data.title);
    await modal.getByLabel("Description").fill(data.description);
    await modal.getByLabel("Street").fill(data.street);
    await modal.getByLabel("City").fill(data.city);
    await modal.getByLabel("State").fill(data.state);
    await modal.getByLabel("Zip code").fill(data.zipCode);
    await modal.getByLabel("Latitude").fill(data.latitude);
    await modal.getByLabel("Longitude").fill(data.longitude);
    // Customer is a <select> of real customers (a picker, not a free-text
    // GUID field — see CustomerSelect) — select by the id global-setup seeded.
    await modal.getByLabel("Customer").selectOption(data.customerId);
  }

  async submitCreateJobForm(): Promise<void> {
    await this.createJobModal.locator('button[type="submit"]').click();
    await expect(this.createJobModal).toBeHidden();
  }

  jobRow(title: string): Locator {
    return this.jobsTable.locator('[data-testid="job-row"]', { hasText: title });
  }

  async waitForJobRow(title: string): Promise<void> {
    await expect(this.jobRow(title)).toBeVisible();
  }

  async toggleStatusFilter(status: string): Promise<void> {
    await this.filterBar.locator(`label:has-text("${status}") input[type="checkbox"]`).click();
  }

  async openCompleteModalFor(title: string): Promise<void> {
    await this.jobRow(title).getByTestId("complete-job-button").click();
    await expect(this.completeJobModal).toBeVisible();
  }

  async fillSignatureUrl(url: string): Promise<void> {
    await this.completeJobModal.getByLabel("Signature URL").fill(url);
  }

  async submitCompleteJobForm(): Promise<void> {
    await this.completeJobModal.locator('button[type="submit"]').click();
    await expect(this.completeJobModal).toBeHidden();
  }

  statusBadgeFor(title: string): Locator {
    return this.jobRow(title).getByTestId("job-status-badge");
  }
}
