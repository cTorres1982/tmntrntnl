import { describe, expect, test } from "vitest";
import { getJobSummary } from "./get-job-summary";
import type { JobState } from "./job-state.type";

describe("getJobSummary", () => {
  test("Draft without notes", () => {
    const state: JobState = { status: "Draft" };
    expect(getJobSummary(state)).toBe("Draft (no notes yet)");
  });

  test("Draft with notes", () => {
    const state: JobState = { status: "Draft", notes: "waiting on customer" };
    expect(getJobSummary(state)).toBe("Draft: waiting on customer");
  });

  test("Scheduled", () => {
    const state: JobState = {
      status: "Scheduled",
      scheduledDate: new Date("2026-06-01"),
      assigneeId: "crew-1",
    };
    expect(getJobSummary(state)).toContain("crew-1");
  });

  test("InProgress", () => {
    const state: JobState = {
      status: "InProgress",
      startedAt: new Date("2026-06-01"),
      assigneeId: "crew-1",
      photos: ["a.jpg", "b.jpg"],
    };
    expect(getJobSummary(state)).toContain("2 photo(s)");
  });

  test("Completed", () => {
    const state: JobState = {
      status: "Completed",
      startedAt: new Date("2026-06-01"),
      completedAt: new Date("2026-06-02"),
      assigneeId: "crew-1",
      photos: [],
      signatureUrl: "https://example.com/signature.png",
    };
    expect(getJobSummary(state)).toContain("crew-1");
  });

  test("Cancelled", () => {
    const state: JobState = { status: "Cancelled", cancelledAt: new Date("2026-06-01"), reason: "no access" };
    expect(getJobSummary(state)).toContain("no access");
  });
});
