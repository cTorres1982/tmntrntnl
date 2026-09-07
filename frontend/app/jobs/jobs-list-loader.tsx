import "server-only";
import { container } from "@/core/infrastructure/di-container";
import { JobsClient } from "@/presentation/views/jobs";

/**
 * A nested async Server Component, kept separate from page.tsx: page.tsx must
 * return synchronously (no top-level await) for its <Suspense> fallback to
 * ever actually show — the fetch has to happen in a component *inside* the
 * boundary, not be awaited by the boundary's parent before it renders.
 */
export async function JobsListLoader() {
  const result = await container.getJobsUseCase.execute({ page: 1, pageSize: 20 });

  return (
    <JobsClient
      initialJobs={result.items}
      initialPagination={{ page: result.page, pageSize: result.pageSize, totalCount: result.totalCount }}
    />
  );
}
