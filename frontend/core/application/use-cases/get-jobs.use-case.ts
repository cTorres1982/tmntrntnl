import type { PagedResult } from "@/core/application/dtos/paged-result.type";
import type { SearchJobsCriteria } from "@/core/application/dtos/search-jobs-criteria.type";
import type { JobRepositoryPort } from "@/core/application/ports/job-repository.port";
import type { JobListItem } from "@/core/domain/job-list-item.type";

/**
 * The "use case from the DI container" app/jobs/page.tsx fetches through
 * (AC.md 2.1.1) — a thin wrapper over the port today, but it's the seam where
 * cross-cutting read concerns (caching, authorization checks, logging) would
 * go without page.tsx or the repository needing to know about them.
 */
export class GetJobsUseCase {
  constructor(private readonly jobRepository: JobRepositoryPort) {}

  execute(criteria: SearchJobsCriteria): Promise<PagedResult<JobListItem>> {
    return this.jobRepository.search(criteria);
  }
}
