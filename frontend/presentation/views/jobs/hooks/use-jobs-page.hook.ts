"use client";

import { useQueryClient } from "@tanstack/react-query";
import { useCallback, useEffect, useMemo, useState } from "react";
import type { JobListItem } from "@/core/domain/job-list-item.type";
import { useFilterJobs } from "../features/filter-jobs";
import { useFilteredJobs, useJobsStoreActions } from "../store/jobs-store.selectors";
import type { Pagination } from "../store/pagination.type";

/**
 * Orchestrates the view's feature slices (AC.md 2.1's "hooks/use-jobs-page.hook.ts
 * -- orchestrates slices") — JobsClient itself stays a thin shell that only
 * reads this hook's return value.
 */
export function useJobsPage(initialJobs: JobListItem[], initialPagination: Pagination) {
  const { setJobs } = useJobsStoreActions();
  const queryClient = useQueryClient();

  // Seed the store once with the Server Component's own fetch so the first
  // client render matches what was already server-rendered, with no flash of
  // empty state while React Query's own request is in flight.
  useEffect(() => {
    setJobs(initialJobs, initialPagination);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const { filters, pagination, isLoading, isError, setFilters, setPage } = useFilterJobs();
  const jobs = useFilteredJobs();

  // useMemo for derived state (AC.md 2.3.4) — recomputed only when the
  // visible job list actually changes, not on every render.
  const totalPhotoCount = useMemo(() => jobs.reduce((sum, job) => sum + job.photoCount, 0), [jobs]);

  const refetchJobs = useCallback(() => {
    queryClient.invalidateQueries({ queryKey: ["jobs"] });
  }, [queryClient]);

  const [isCreateModalOpen, setCreateModalOpen] = useState(false);
  const [completingJobId, setCompletingJobId] = useState<string | null>(null);

  return {
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
    openCreateModal: () => setCreateModalOpen(true),
    closeCreateModal: () => setCreateModalOpen(false),
    completingJobId,
    openCompleteModal: (jobId: string) => setCompletingJobId(jobId),
    closeCompleteModal: () => setCompletingJobId(null),
  };
}
