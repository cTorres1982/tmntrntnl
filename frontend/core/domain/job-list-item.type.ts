import type { JobState } from "./job-state.type";

/**
 * Flat shape returned by the backend's SearchJobsQuery (JobResponse) — a list/
 * search projection, not the rich state-dependent shape from job-state.type.ts.
 * Deliberately a separate type: a list row always carries every column
 * regardless of status, which is exactly what the JobState discriminated
 * union is designed to prevent for state-dependent domain logic.
 */
export interface JobListItem {
  id: string;
  title: string;
  description: string;
  status: JobState["status"];
  scheduledDate: string | null;
  assigneeId: string | null;
  customerId: string;
  street: string;
  city: string;
  state: string;
  zipCode: string;
  photoCount: number;
  createdAtUtc: string;
}
