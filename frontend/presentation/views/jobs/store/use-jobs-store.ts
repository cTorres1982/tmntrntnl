import { create } from "zustand";
import type { JobListItem } from "@/core/domain/job-list-item.type";
import type { JobsStoreState } from "./jobs-store.type";

export const useJobsStore = create<JobsStoreState>((set, get) => ({
  jobs: [],
  selectedJobIds: new Set<string>(),
  filters: { searchTerm: "", statuses: [], fromDate: null, toDate: null },
  pagination: { page: 1, pageSize: 20, totalCount: 0 },
  sortConfig: null,

  setJobs: (jobs, pagination) => set({ jobs, pagination }),

  setFilters: (filters) => set((state) => ({ filters: { ...state.filters, ...filters } })),

  setSortConfig: (sortConfig) => set({ sortConfig }),

  setPage: (page) => set((state) => ({ pagination: { ...state.pagination, page } })),

  toggleJobSelection: (jobId) =>
    set((state) => {
      const next = new Set(state.selectedJobIds);
      if (next.has(jobId)) {
        next.delete(jobId);
      } else {
        next.add(jobId);
      }
      return { selectedJobIds: next };
    }),

  clearSelection: () => set({ selectedJobIds: new Set<string>() }),

  applyOptimisticStatus: (jobId, status) => {
    const previous = get().jobs.find((job) => job.id === jobId);
    if (!previous) return undefined;

    set((state) => ({
      jobs: state.jobs.map((job) => (job.id === jobId ? { ...job, status } : job)),
    }));

    return previous;
  },

  rollbackJob: (jobId, previous) =>
    set((state) => ({
      jobs: state.jobs.map((job): JobListItem => (job.id === jobId ? previous : job)),
    })),
}));
