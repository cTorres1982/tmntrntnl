import type { JobListItem } from "@/core/domain/job-list-item.type";
import type { JobFilters } from "./job-filters.type";
import type { Pagination } from "./pagination.type";
import type { SortConfig } from "./sort-config.type";

/**
 * Client-side UI state only (AC.md 2.2.5) — `jobs` is the browser's current
 * working copy of whatever page the server last returned (seeded from the
 * Server Component's initial props, replaced wholesale by useFilterJobs after
 * each React Query fetch), not an independent cache duplicating server state.
 * Filtering by status/date/search triggers a real query (server-side, via
 * React Query) because it affects pagination and needs the backend's indexes;
 * `selectFilteredJobs` only re-sorts the already-fetched page — an operation
 * that's genuinely free to do client-side.
 */
export interface JobsStoreState {
  jobs: JobListItem[];
  selectedJobIds: Set<string>;
  filters: JobFilters;
  pagination: Pagination;
  sortConfig: SortConfig | null;

  setJobs: (jobs: JobListItem[], pagination: Pagination) => void;
  setFilters: (filters: Partial<JobFilters>) => void;
  setSortConfig: (sortConfig: SortConfig | null) => void;
  setPage: (page: number) => void;
  toggleJobSelection: (jobId: string) => void;
  clearSelection: () => void;

  /** Optimistic update: apply immediately, keep the previous value to roll back if the Server Action fails. */
  applyOptimisticStatus: (jobId: string, status: JobListItem["status"]) => JobListItem | undefined;
  rollbackJob: (jobId: string, previous: JobListItem) => void;
}
