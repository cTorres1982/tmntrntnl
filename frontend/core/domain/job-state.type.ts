/**
 * Discriminant values match the C# JobStatus enum's string representation
 * (backend/.../Jobs.Domain/JobStatus.cs, stored as text and serialized as-is
 * over the API) — the frontend and backend agree on the same wire values.
 */
export type JobState =
  | { status: "Draft"; notes?: string }
  | { status: "Scheduled"; scheduledDate: Date; assigneeId: string }
  | { status: "InProgress"; startedAt: Date; assigneeId: string; photos: string[] }
  | {
      status: "Completed";
      startedAt: Date;
      completedAt: Date;
      assigneeId: string;
      photos: string[];
      signatureUrl: string;
    }
  | { status: "Cancelled"; cancelledAt: Date; reason: string };
