import { describe, expect, test } from "vitest";
import type { JobState } from "./job-state.type";
import { transitionJob } from "./transition-job";

describe("transitionJob — valid transitions", () => {
  test("Draft --schedule--> Scheduled", () => {
    const draft: JobState = { status: "Draft" };

    const result = transitionJob(draft, {
      type: "schedule",
      scheduledDate: new Date("2026-06-01"),
      assigneeId: "crew-1",
    });

    expect(result).toEqual({
      status: "Scheduled",
      scheduledDate: new Date("2026-06-01"),
      assigneeId: "crew-1",
    });
  });

  test("Scheduled --start--> InProgress, carrying assigneeId forward", () => {
    const scheduled: JobState = {
      status: "Scheduled",
      scheduledDate: new Date("2026-06-01"),
      assigneeId: "crew-1",
    };

    const result = transitionJob(scheduled, { type: "start", startedAt: new Date("2026-06-01T09:00:00") });

    expect(result).toEqual({
      status: "InProgress",
      startedAt: new Date("2026-06-01T09:00:00"),
      assigneeId: "crew-1",
      photos: [],
    });
  });

  test("Scheduled --cancel--> Cancelled", () => {
    const scheduled: JobState = {
      status: "Scheduled",
      scheduledDate: new Date("2026-06-01"),
      assigneeId: "crew-1",
    };

    const result = transitionJob(scheduled, {
      type: "cancel",
      cancelledAt: new Date("2026-05-30"),
      reason: "customer request",
    });

    expect(result).toEqual({
      status: "Cancelled",
      cancelledAt: new Date("2026-05-30"),
      reason: "customer request",
    });
  });

  test("InProgress --complete--> Completed, carrying startedAt/assigneeId/photos forward", () => {
    const inProgress: JobState = {
      status: "InProgress",
      startedAt: new Date("2026-06-01T09:00:00"),
      assigneeId: "crew-1",
      photos: ["front.jpg"],
    };

    const result = transitionJob(inProgress, {
      type: "complete",
      completedAt: new Date("2026-06-01T15:00:00"),
      signatureUrl: "https://example.com/signature.png",
    });

    expect(result).toEqual({
      status: "Completed",
      startedAt: new Date("2026-06-01T09:00:00"),
      completedAt: new Date("2026-06-01T15:00:00"),
      assigneeId: "crew-1",
      photos: ["front.jpg"],
      signatureUrl: "https://example.com/signature.png",
    });
  });

  test("InProgress --cancel--> Cancelled", () => {
    const inProgress: JobState = {
      status: "InProgress",
      startedAt: new Date("2026-06-01T09:00:00"),
      assigneeId: "crew-1",
      photos: [],
    };

    const result = transitionJob(inProgress, {
      type: "cancel",
      cancelledAt: new Date("2026-06-01T10:00:00"),
      reason: "unsafe conditions",
    });

    expect(result).toEqual({
      status: "Cancelled",
      cancelledAt: new Date("2026-06-01T10:00:00"),
      reason: "unsafe conditions",
    });
  });
});

describe("transitionJob — invalid transitions are compile-time errors", () => {
  test("type-level only: see @ts-expect-error assertions below", () => {
    const draft: JobState = { status: "Draft" };
    const completed: JobState = {
      status: "Completed",
      startedAt: new Date(),
      completedAt: new Date(),
      assigneeId: "crew-1",
      photos: [],
      signatureUrl: "https://example.com/signature.png",
    };
    const cancelled: JobState = { status: "Cancelled", cancelledAt: new Date(), reason: "n/a" };

    // The ts-expect-error comments below suppress the compile error so this
    // file itself can compile, but the calls still run at runtime —
    // transitionJob's fallback throw is what actually executes, so each of
    // these is a genuine belt-and-suspenders check: rejected at compile time
    // *and* at runtime.
    if (draft.status === "Draft") {
      expect(() => {
        // @ts-expect-error Draft cannot go straight to Completed
        transitionJob(draft, { type: "complete", completedAt: new Date(), signatureUrl: "x" });
      }).toThrow(/Draft/);
    }

    if (completed.status === "Completed") {
      expect(() => {
        // @ts-expect-error Completed is terminal — no action is valid from it
        transitionJob(completed, { type: "cancel", cancelledAt: new Date(), reason: "n/a" });
      }).toThrow(/Completed/);
    }

    if (cancelled.status === "Cancelled") {
      expect(() => {
        // @ts-expect-error Cancelled is terminal — no action is valid from it
        transitionJob(cancelled, { type: "schedule", scheduledDate: new Date(), assigneeId: "crew-1" });
      }).toThrow(/Cancelled/);
    }
  });
});
