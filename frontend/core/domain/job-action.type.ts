/** The actions transitionJob accepts — not specified by name in AC.md, only implied by "transitionJob(current: JobState, action: JobAction)"; designed to carry exactly the data each resulting JobState variant needs. */
export type JobAction =
  | { type: "schedule"; scheduledDate: Date; assigneeId: string }
  | { type: "start"; startedAt: Date }
  | { type: "complete"; completedAt: Date; signatureUrl: string }
  | { type: "cancel"; cancelledAt: Date; reason: string };
