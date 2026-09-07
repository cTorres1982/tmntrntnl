import type { JobState } from "@/core/domain/job-state.type";

export interface JobFilters {
  searchTerm: string;
  statuses: JobState["status"][];
  fromDate: string | null;
  toDate: string | null;
}
