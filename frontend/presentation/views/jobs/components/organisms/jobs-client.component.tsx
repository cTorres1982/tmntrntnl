"use client";

import { ErrorBoundary } from "@/shared/components/error-boundary.component";
import type { JobListItem } from "@/core/domain/job-list-item.type";
import { CompleteJobModal } from "../../features/complete-job";
import { CreateJobModal } from "../../features/create-job";
import { JobFilterBar } from "../../features/filter-jobs";
import { useJobsPage } from "../../hooks/use-jobs-page.hook";
import type { Pagination } from "../../store/pagination.type";
import { JobsListSkeleton } from "../molecules/jobs-list-skeleton.component";
import { JobsTable } from "../molecules/jobs-table.component";

interface JobsClientProps {
  initialJobs: JobListItem[];
  initialPagination: Pagination;
}

/**
 * Thin shell (AC.md 2.1.6): every piece of state and every handler comes from
 * useJobsPage or the feature hooks it composes — this component only wires
 * hook output to JSX.
 */
export function JobsClient({ initialJobs, initialPagination }: JobsClientProps) {
  const {
    jobs,
    totalPhotoCount,
    filters,
    pagination,
    isLoading,
    isError,
    setFilters,
    setPage,
    refetchJobs,
    isCreateModalOpen,
    openCreateModal,
    closeCreateModal,
    completingJobId,
    openCompleteModal,
    closeCompleteModal,
  } = useJobsPage(initialJobs, initialPagination);

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Jobs</h1>
        <button
          type="button"
          onClick={openCreateModal}
          className="rounded-md bg-blue-600 px-3 py-1.5 text-sm text-white"
          data-testid="open-create-job-button"
        >
          Create job
        </button>
      </div>

      <JobFilterBar>
        <JobFilterBar.Search
          value={filters.searchTerm}
          onChange={(searchTerm) => setFilters({ searchTerm })}
        />
        <JobFilterBar.Status value={filters.statuses} onChange={(statuses) => setFilters({ statuses })} />
        <JobFilterBar.DateRange
          fromDate={filters.fromDate}
          toDate={filters.toDate}
          onChange={({ fromDate, toDate }) => setFilters({ fromDate, toDate })}
        />
      </JobFilterBar>

      <p className="text-sm text-gray-500">{totalPhotoCount} photo(s) across the current view</p>

      <ErrorBoundary
        fallback={(error, reset) => (
          <div className="rounded-md border border-red-200 bg-red-50 p-4 text-sm text-red-700">
            <p>Something went wrong rendering the job list: {error.message}</p>
            <button type="button" onClick={reset} className="mt-2 underline">
              Try again
            </button>
          </div>
        )}
      >
        {isLoading ? (
          <JobsListSkeleton />
        ) : isError ? (
          <p className="text-sm text-red-600">Failed to load jobs. Please try again.</p>
        ) : (
          <JobsTable jobs={jobs} onCompleteJob={openCompleteModal} />
        )}
      </ErrorBoundary>

      <div className="flex items-center justify-between text-sm text-gray-500">
        <span>
          Page {pagination.page} of {Math.max(1, Math.ceil(pagination.totalCount / pagination.pageSize))}
        </span>
        <div className="flex gap-2">
          <button
            type="button"
            disabled={pagination.page <= 1}
            onClick={() => setPage(pagination.page - 1)}
            className="disabled:opacity-40"
          >
            Previous
          </button>
          <button
            type="button"
            disabled={pagination.page * pagination.pageSize >= pagination.totalCount}
            onClick={() => setPage(pagination.page + 1)}
            className="disabled:opacity-40"
          >
            Next
          </button>
        </div>
      </div>

      <CreateJobModal open={isCreateModalOpen} onClose={closeCreateModal} onCreated={refetchJobs} />
      <CompleteJobModal jobId={completingJobId} onClose={closeCompleteModal} onCompleted={refetchJobs} />
    </div>
  );
}
