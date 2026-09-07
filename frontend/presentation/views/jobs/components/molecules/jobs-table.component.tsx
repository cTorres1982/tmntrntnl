import type { JobListItem } from "@/core/domain/job-list-item.type";
import { JobStatusBadge } from "../atoms/job-status-badge.component";

interface JobsTableProps {
  jobs: JobListItem[];
  onCompleteJob: (jobId: string) => void;
}

export function JobsTable({ jobs, onCompleteJob }: JobsTableProps) {
  return jobs.length === 0 ? (
    <p className="py-8 text-center text-sm text-gray-500" data-testid="jobs-empty-state">
      No jobs match the current filters.
    </p>
  ) : (
    <table className="w-full text-left text-sm" data-testid="jobs-table">
      <thead>
        <tr className="border-b border-gray-200 text-gray-500">
          <th className="py-2">Title</th>
          <th className="py-2">Status</th>
          <th className="py-2">Scheduled</th>
          <th className="py-2">Photos</th>
          <th className="py-2" />
        </tr>
      </thead>
      <tbody>
        {jobs.map((job) => (
          <tr key={job.id} className="border-b border-gray-100" data-testid="job-row">
            <td className="py-2">{job.title}</td>
            <td className="py-2">
              <JobStatusBadge status={job.status} />
            </td>
            <td className="py-2">{job.scheduledDate ? new Date(job.scheduledDate).toLocaleDateString() : "—"}</td>
            <td className="py-2">{job.photoCount}</td>
            <td className="py-2 text-right">
              {job.status === "InProgress" ? (
                <button
                  type="button"
                  onClick={() => onCompleteJob(job.id)}
                  className="text-sm text-blue-600 hover:underline"
                  data-testid="complete-job-button"
                >
                  Complete
                </button>
              ) : null}
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
