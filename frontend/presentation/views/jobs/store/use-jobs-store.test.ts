import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, test } from "vitest";
import type { JobListItem } from "@/core/domain/job-list-item.type";
import { useFilteredJobs, useJobFilters, useJobsPagination, useJobsStoreActions } from "./jobs-store.selectors";
import { useJobsStore } from "./use-jobs-store";

const initialState = useJobsStore.getState();

function resetStore() {
  useJobsStore.setState(initialState, true);
}

function buildJob(overrides: Partial<JobListItem>): JobListItem {
  return {
    id: "job-1",
    title: "Roof repair",
    description: "desc",
    status: "Draft",
    scheduledDate: null,
    assigneeId: null,
    customerId: "cust-1",
    street: "123 Main St",
    city: "Austin",
    state: "TX",
    zipCode: "78701",
    photoCount: 0,
    createdAtUtc: "2026-01-01T00:00:00Z",
    ...overrides,
  };
}

describe("useJobsStore", () => {
  beforeEach(() => {
    resetStore();
  });

  test("setJobs replaces jobs and pagination", () => {
    const jobs = [buildJob({ id: "job-1" }), buildJob({ id: "job-2" })];
    act(() => useJobsStore.getState().setJobs(jobs, { page: 2, pageSize: 10, totalCount: 2 }));

    expect(useJobsStore.getState().jobs).toEqual(jobs);
    expect(useJobsStore.getState().pagination).toEqual({ page: 2, pageSize: 10, totalCount: 2 });
  });

  test("useFilteredJobs (selector) sorts by sortConfig without mutating the underlying jobs order in state", () => {
    const jobs = [buildJob({ id: "b", title: "Bravo" }), buildJob({ id: "a", title: "Alpha" })];
    act(() => {
      useJobsStore.getState().setJobs(jobs, { page: 1, pageSize: 20, totalCount: 2 });
      useJobsStore.getState().setSortConfig({ field: "title", direction: "asc" });
    });

    const { result } = renderHook(() => useFilteredJobs());

    expect(result.current.map((job) => job.title)).toEqual(["Alpha", "Bravo"]);
    // The selector derives a sorted copy — it must not mutate state.jobs itself.
    expect(useJobsStore.getState().jobs.map((job) => job.title)).toEqual(["Bravo", "Alpha"]);
  });

  test("useFilteredJobs (selector) returns jobs as-is when sortConfig is null", () => {
    const jobs = [buildJob({ id: "b", title: "Bravo" }), buildJob({ id: "a", title: "Alpha" })];
    act(() => useJobsStore.getState().setJobs(jobs, { page: 1, pageSize: 20, totalCount: 2 }));

    const { result } = renderHook(() => useFilteredJobs());

    expect(result.current.map((job) => job.title)).toEqual(["Bravo", "Alpha"]);
  });

  test("useJobFilters/useJobsPagination (selectors) each expose only their own slice of state", () => {
    act(() => useJobsStore.getState().setFilters({ searchTerm: "roof" }));

    const filters = renderHook(() => useJobFilters());
    const pagination = renderHook(() => useJobsPagination());

    expect(filters.result.current.searchTerm).toBe("roof");
    expect(pagination.result.current).toEqual(initialState.pagination);
  });

  test("applyOptimisticStatus updates the job immediately and returns the previous row", () => {
    const job = buildJob({ id: "job-1", status: "InProgress" });
    act(() => useJobsStore.getState().setJobs([job], { page: 1, pageSize: 20, totalCount: 1 }));

    let previous: JobListItem | undefined;
    act(() => {
      previous = useJobsStore.getState().applyOptimisticStatus("job-1", "Completed");
    });

    expect(previous?.status).toBe("InProgress");
    expect(useJobsStore.getState().jobs[0].status).toBe("Completed");
  });

  test("applyOptimisticStatus is a no-op and returns undefined for a job not in the store", () => {
    const previous = useJobsStore.getState().applyOptimisticStatus("missing-job", "Completed");
    expect(previous).toBeUndefined();
  });

  test("rollbackJob restores the exact previous row after a failed mutation", () => {
    const original = buildJob({ id: "job-1", status: "InProgress" });
    act(() => useJobsStore.getState().setJobs([original], { page: 1, pageSize: 20, totalCount: 1 }));
    act(() => {
      useJobsStore.getState().applyOptimisticStatus("job-1", "Completed");
    });
    expect(useJobsStore.getState().jobs[0].status).toBe("Completed");

    act(() => useJobsStore.getState().rollbackJob("job-1", original));

    expect(useJobsStore.getState().jobs[0]).toEqual(original);
  });

  test("useJobsStoreActions (useShallow) returns a stable reference across re-renders", () => {
    const { result, rerender } = renderHook(() => useJobsStoreActions());
    const firstRender = result.current;

    rerender();

    expect(result.current).toBe(firstRender);
  });
});
