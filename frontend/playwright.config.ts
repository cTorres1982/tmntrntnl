import { defineConfig, devices } from "@playwright/test";

/**
 * Requires the .NET backend (and Postgres) already running separately — see
 * README.md. Only the Next.js dev server is started here; spinning up a
 * whole other stack's runtime from a frontend test config would be its own
 * source of flakiness, and docker-compose already owns "run everything."
 */
export default defineConfig({
  testDir: "./e2e",
  fullyParallel: false,
  retries: process.env.CI ? 1 : 0,
  reporter: "html",
  globalSetup: "./e2e/global-setup.ts",
  globalTeardown: "./e2e/global-teardown.ts",
  use: {
    baseURL: "http://localhost:3000",
    trace: "on-first-retry",
    // AC.md 5.3's explicit requirement.
    screenshot: "only-on-failure",
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  webServer: {
    command: "pnpm dev",
    url: "http://localhost:3000",
    reuseExistingServer: !process.env.CI,
    timeout: 60_000,
  },
});
