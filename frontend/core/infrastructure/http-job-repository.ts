import "server-only";
import type { PagedResult } from "@/core/application/dtos/paged-result.type";
import type { SearchJobsCriteria } from "@/core/application/dtos/search-jobs-criteria.type";
import type { JobRepositoryPort } from "@/core/application/ports/job-repository.port";
import type { JobListItem } from "@/core/domain/job-list-item.type";
import { apiConfig } from "./api-config";

export class HttpJobRepository implements JobRepositoryPort {
  async search(criteria: SearchJobsCriteria): Promise<PagedResult<JobListItem>> {
    const params = new URLSearchParams();

    if (criteria.searchTerm) params.set("searchTerm", criteria.searchTerm);
    if (criteria.fromDate) params.set("fromDate", criteria.fromDate);
    if (criteria.toDate) params.set("toDate", criteria.toDate);
    if (criteria.assigneeId) params.set("assigneeId", criteria.assigneeId);
    params.set("page", String(criteria.page ?? 1));
    params.set("pageSize", String(criteria.pageSize ?? 20));
    for (const status of criteria.statuses ?? []) {
      params.append("statuses", status);
    }

    const response = await fetch(`${apiConfig.baseUrl}/jobs?${params.toString()}`, {
      headers: { "X-Organization-Id": apiConfig.devOrganizationId },
      cache: "no-store",
    });

    if (!response.ok) {
      throw new Error(`Failed to search jobs: the backend responded with ${response.status}.`);
    }

    return (await response.json()) as PagedResult<JobListItem>;
  }
}
