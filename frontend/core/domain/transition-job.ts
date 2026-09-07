import type { JobAction } from "./job-action.type";
import type { JobState } from "./job-state.type";

/**
 * One overload per valid (currentStatus, actionType) pair — this is what makes
 * an invalid transition (e.g. Draft + "complete") a compile-time error: it
 * simply matches none of the declared overloads. The permissive
 * `(current: JobState, action: JobAction)` signature below is the
 * implementation signature only; TypeScript hides it from callers whenever
 * overloads are declared, so external code can never call it with a
 * combination the overloads don't allow.
 */
export function transitionJob(
  current: Extract<JobState, { status: "Draft" }>,
  action: Extract<JobAction, { type: "schedule" }>,
): Extract<JobState, { status: "Scheduled" }>;
export function transitionJob(
  current: Extract<JobState, { status: "Scheduled" }>,
  action: Extract<JobAction, { type: "start" }>,
): Extract<JobState, { status: "InProgress" }>;
export function transitionJob(
  current: Extract<JobState, { status: "Scheduled" }>,
  action: Extract<JobAction, { type: "cancel" }>,
): Extract<JobState, { status: "Cancelled" }>;
export function transitionJob(
  current: Extract<JobState, { status: "InProgress" }>,
  action: Extract<JobAction, { type: "complete" }>,
): Extract<JobState, { status: "Completed" }>;
export function transitionJob(
  current: Extract<JobState, { status: "InProgress" }>,
  action: Extract<JobAction, { type: "cancel" }>,
): Extract<JobState, { status: "Cancelled" }>;
export function transitionJob(current: JobState, action: JobAction): JobState {
  if (current.status === "Draft" && action.type === "schedule") {
    return { status: "Scheduled", scheduledDate: action.scheduledDate, assigneeId: action.assigneeId };
  }

  if (current.status === "Scheduled" && action.type === "start") {
    return { status: "InProgress", startedAt: action.startedAt, assigneeId: current.assigneeId, photos: [] };
  }

  if (current.status === "Scheduled" && action.type === "cancel") {
    return { status: "Cancelled", cancelledAt: action.cancelledAt, reason: action.reason };
  }

  if (current.status === "InProgress" && action.type === "complete") {
    return {
      status: "Completed",
      startedAt: current.startedAt,
      completedAt: action.completedAt,
      assigneeId: current.assigneeId,
      photos: current.photos,
      signatureUrl: action.signatureUrl,
    };
  }

  if (current.status === "InProgress" && action.type === "cancel") {
    return { status: "Cancelled", cancelledAt: action.cancelledAt, reason: action.reason };
  }

  // Unreachable through the typed overloads above — only reachable if a
  // caller bypasses the type system (plain JS, `any`, etc.).
  throw new Error(`Invalid transition: cannot apply action "${action.type}" to a job in status "${current.status}".`);
}
