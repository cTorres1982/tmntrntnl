"use client";

import type { ReactNode } from "react";
import type { JobState } from "@/core/domain/job-state.type";

const ALL_STATUSES: JobState["status"][] = ["Draft", "Scheduled", "InProgress", "Completed", "Cancelled"];

function JobFilterBarRoot({ children }: { children: ReactNode }) {
  return (
    <div className="flex flex-wrap items-end gap-4" data-testid="job-filter-bar">
      {children}
    </div>
  );
}

interface StatusFilterProps {
  value: JobState["status"][];
  onChange: (statuses: JobState["status"][]) => void;
}

/** Controlled: `value`/`onChange` come from the parent (useFilterJobs), same pattern as the create-job form fields. */
function StatusFilter({ value, onChange }: StatusFilterProps) {
  function toggle(status: JobState["status"]) {
    onChange(value.includes(status) ? value.filter((s) => s !== status) : [...value, status]);
  }

  return (
    <fieldset className="flex flex-col gap-1 text-sm">
      <legend className="font-medium text-gray-700">Status</legend>
      <div className="flex flex-wrap gap-2">
        {ALL_STATUSES.map((status) => (
          <label key={status} className="flex items-center gap-1">
            <input type="checkbox" checked={value.includes(status)} onChange={() => toggle(status)} />
            {status}
          </label>
        ))}
      </div>
    </fieldset>
  );
}

interface DateRangeFilterProps {
  fromDate: string | null;
  toDate: string | null;
  onChange: (range: { fromDate: string | null; toDate: string | null }) => void;
}

function DateRangeFilter({ fromDate, toDate, onChange }: DateRangeFilterProps) {
  return (
    <div className="flex flex-col gap-1 text-sm">
      <span className="font-medium text-gray-700">Scheduled date</span>
      <div className="flex gap-2">
        <input
          type="date"
          className="rounded-md border border-gray-300 px-2 py-1"
          value={fromDate ?? ""}
          onChange={(event) => onChange({ fromDate: event.target.value || null, toDate })}
        />
        <input
          type="date"
          className="rounded-md border border-gray-300 px-2 py-1"
          value={toDate ?? ""}
          onChange={(event) => onChange({ fromDate, toDate: event.target.value || null })}
        />
      </div>
    </div>
  );
}

interface SearchFilterProps {
  value: string;
  onChange: (value: string) => void;
}

function SearchFilter({ value, onChange }: SearchFilterProps) {
  return (
    <label className="flex flex-col gap-1 text-sm">
      <span className="font-medium text-gray-700">Search</span>
      <input
        type="search"
        className="rounded-md border border-gray-300 px-2 py-1"
        value={value}
        onChange={(event) => onChange(event.target.value)}
      />
    </label>
  );
}

/** Compound Component pattern (AC.md 2.3.2): `<JobFilterBar><JobFilterBar.Status />...</JobFilterBar>`. */
export const JobFilterBar = Object.assign(JobFilterBarRoot, {
  Status: StatusFilter,
  DateRange: DateRangeFilter,
  Search: SearchFilter,
});
