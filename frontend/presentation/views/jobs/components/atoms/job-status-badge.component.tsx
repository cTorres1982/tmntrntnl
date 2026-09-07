import type { JobState } from "@/core/domain/job-state.type";

const STATUS_STYLES: Record<JobState["status"], string> = {
  Draft: "bg-gray-100 text-gray-700",
  Scheduled: "bg-blue-100 text-blue-700",
  InProgress: "bg-amber-100 text-amber-700",
  Completed: "bg-green-100 text-green-700",
  Cancelled: "bg-red-100 text-red-700",
};

interface JobStatusBadgeProps {
  status: JobState["status"];
}

export function JobStatusBadge({ status }: JobStatusBadgeProps) {
  return (
    <span
      className={`inline-block rounded-full px-2 py-0.5 text-xs font-medium ${STATUS_STYLES[status]}`}
      data-testid="job-status-badge"
    >
      {status === "InProgress" ? "In Progress" : status}
    </span>
  );
}
