"use client";

import { useQuery } from "@tanstack/react-query";
import { useEffect } from "react";
import type { PagedResult } from "@/core/application/dtos/paged-result.type";
import type { JobListItem } from "@/core/domain/job-list-item.type";
import { useJobFilters, useJobsPagination, useJobsStoreActions } from "../../../store/jobs-store.selectors";
import type { JobFilters } from "../../../store/job-filters.type";

function buildSearchParams(filters: JobFilters, page: number, pageSize: number): URLSearchParams {
  const params = new URLSearchParams();
  if (filters.searchTerm) params.set("searchTerm", filters.searchTerm);
  if (filters.fromDate) params.set("fromDate", filters.fromDate);
  if (filters.toDate) params.set("toDate", filters.toDate);
  for (const status of filters.statuses) params.append("statuses", status);
  params.set("page", String(page));
  params.set("pageSize", String(pageSize));
  return params;
}

/**
 * Client-side data fetching via React Query (AC.md 2.1.7's explicit
 * allowance) against the /api/jobs BFF route — filtering re-queries the
 * backend rather than filtering in memory, since only the current page is
 * ever loaded client-side and status/date filters affect the total count.
 */
export function useFilterJobs() {
  const filters = useJobFilters();
  const pagination = useJobsPagination();
  const { setJobs, setFilters, setPage } = useJobsStoreActions();

  const params = buildSearchParams(filters, pagination.page, pagination.pageSize);
  const queryKey = params.toString();

  const query = useQuery({
    queryKey: ["jobs", queryKey],
    queryFn: async (): Promise<PagedResult<JobListItem>> => {
      const response = await fetch(`/api/jobs?${queryKey}`);
      if (!response.ok) {
        throw new Error(`Failed to load jobs (${response.status}).`);
      }
      return (await response.json()) as PagedResult<JobListItem>;
    },
  });

  // Syncing React Query's cache into the Zustand store — two independent
  // state containers being reconciled, not a value derived from existing
  // state, which is what AC.md 2.2.3 asks to avoid useEffect for (see
  // selectFilteredJobs, which does that derivation as a plain selector).
  useEffect(() => {
    if (query.data) {
      setJobs(query.data.items, {
        page: query.data.page,
        pageSize: query.data.pageSize,
        totalCount: query.data.totalCount,
      });
    }
  }, [query.data, setJobs]);

  return {
    filters,
    pagination,
    isLoading: query.isLoading,
    isError: query.isError,
    setFilters,
    setPage,
  };
}
