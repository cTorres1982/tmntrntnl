import type { JobState } from "./job-state.type";

/**
 * If a new JobState variant is ever added without a matching case here, the
 * `default` branch's `state` is no longer `never` (some variant would still
 * be un-narrowed), so the assignment to `exhaustiveCheck: never` fails to
 * compile — exhaustiveness enforced by the type checker, not by remembering
 * to update this function.
 */
export function getJobSummary(state: JobState): string {
  switch (state.status) {
    case "Draft":
      return state.notes ? `Draft: ${state.notes}` : "Draft (no notes yet)";
    case "Scheduled":
      return `Scheduled for ${state.scheduledDate.toLocaleDateString()}, assigned to ${state.assigneeId}`;
    case "InProgress":
      return `In progress since ${state.startedAt.toLocaleDateString()} (${state.photos.length} photo(s) so far)`;
    case "Completed":
      return `Completed on ${state.completedAt.toLocaleDateString()}, signed off by ${state.assigneeId}`;
    case "Cancelled":
      return `Cancelled on ${state.cancelledAt.toLocaleDateString()}: ${state.reason}`;
    default: {
      const exhaustiveCheck: never = state;
      throw new Error(`Unhandled job status: ${JSON.stringify(exhaustiveCheck)}`);
    }
  }
}
