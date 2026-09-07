import type { PagedResult } from "@/core/application/dtos/paged-result.type";
import type { SearchJobsCriteria } from "@/core/application/dtos/search-jobs-criteria.type";
import type { JobListItem } from "@/core/domain/job-list-item.type";

/**
 * Port the Application layer depends on; Infrastructure (HttpJobRepository)
 * implements it. Mirrors the backend's own IJobRepository/IJobsDbContext
 * split in spirit — this is the read side only, since the frontend's writes
 * go through Server Actions calling the backend directly, not through a
 * client-side "repository" abstraction.
 */
export interface JobRepositoryPort {
  search(criteria: SearchJobsCriteria): Promise<PagedResult<JobListItem>>;
}
