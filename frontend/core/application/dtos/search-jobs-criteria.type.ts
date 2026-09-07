import type { JobState } from "@/core/domain/job-state.type";

export interface SearchJobsCriteria {
  searchTerm?: string;
  statuses?: JobState["status"][];
  fromDate?: string;
  toDate?: string;
  assigneeId?: string;
  page?: number;
  pageSize?: number;
}
