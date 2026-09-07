import { useShallow } from "zustand/react/shallow";
import type { JobListItem } from "@/core/domain/job-list-item.type";
import type { JobsStoreState } from "./jobs-store.type";
import { useJobsStore } from "./use-jobs-store";

/**
 * Derives the sorted view of the current page — status/date/search filtering
 * happens server-side via React Query (see useFilterJobs), so this selector
 * only re-sorts what's already loaded. A selector, not useEffect + setState
 * (AC.md 2.2.3): it recomputes during render from existing state, so there is
 * no extra render pass and no risk of the sorted copy drifting out of sync
 * with `jobs`.
 */
function selectFilteredJobs(state: JobsStoreState): JobListItem[] {
  const { jobs, sortConfig } = state;
  if (!sortConfig) return jobs;

  const direction = sortConfig.direction === "asc" ? 1 : -1;
  return [...jobs].sort((a, b) => {
    const aValue = a[sortConfig.field] ?? "";
    const bValue = b[sortConfig.field] ?? "";
    if (aValue < bValue) return -direction;
    if (aValue > bValue) return direction;
    return 0;
  });
}

// Each hook below selects exactly the slice a component needs, so a
// component reading only `filters` (say) doesn't re-render when `jobs`
// changes — the point of "uses selectors" in AC.md 2.2.2.
export const useJobs = () => useJobsStore((state) => state.jobs);
export const useFilteredJobs = () => useJobsStore(selectFilteredJobs);
export const useSelectedJobIds = () => useJobsStore((state) => state.selectedJobIds);
export const useJobFilters = () => useJobsStore((state) => state.filters);
export const useJobsPagination = () => useJobsStore((state) => state.pagination);
export const useJobsSortConfig = () => useJobsStore((state) => state.sortConfig);

/**
 * Action references are stable for the store's lifetime, but bundling
 * several into one object literal selector would normally still return a
 * *new* wrapper object every call, defeating memoization — useShallow
 * compares the returned object's values instead of its identity, so this
 * stays a single stable reference across renders even though it's declared
 * as an object literal.
 */
export const useJobsStoreActions = () =>
  useJobsStore(
    useShallow((state) => ({
      setJobs: state.setJobs,
      setFilters: state.setFilters,
      setSortConfig: state.setSortConfig,
      setPage: state.setPage,
      toggleJobSelection: state.toggleJobSelection,
      clearSelection: state.clearSelection,
      applyOptimisticStatus: state.applyOptimisticStatus,
      rollbackJob: state.rollbackJob,
    })),
  );
