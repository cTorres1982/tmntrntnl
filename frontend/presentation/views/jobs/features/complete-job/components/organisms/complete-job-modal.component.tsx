"use client";

import { useCompleteJob } from "../../hooks/use-complete-job.hook";

interface CompleteJobModalProps {
  jobId: string | null;
  onClose: () => void;
  onCompleted: () => void;
}

export function CompleteJobModal({ jobId, onClose, onCompleted }: CompleteJobModalProps) {
  const { signatureUrl, setSignatureUrl, isSubmitting, error, onSubmit } = useCompleteJob(jobId ?? "", () => {
    onCompleted();
    onClose();
  });

  return jobId === null ? null : (
    <div className="fixed inset-0 flex items-center justify-center bg-black/40" data-testid="complete-job-modal">
      <form onSubmit={onSubmit} className="flex w-full max-w-md flex-col gap-3 rounded-lg bg-white p-6">
        <h2 className="text-lg font-semibold">Complete job</h2>

        <label className="flex flex-col gap-1 text-sm">
          <span className="font-medium text-gray-700">Signature URL</span>
          <input
            className="rounded-md border border-gray-300 px-2 py-1"
            type="url"
            required
            value={signatureUrl}
            onChange={(event) => setSignatureUrl(event.target.value)}
          />
        </label>

        {error ? <p className="text-sm text-red-600">{error}</p> : null}

        <div className="mt-2 flex justify-end gap-2">
          <button type="button" onClick={onClose} className="rounded-md px-3 py-1.5 text-sm">
            Cancel
          </button>
          <button
            type="submit"
            disabled={isSubmitting}
            className="rounded-md bg-blue-600 px-3 py-1.5 text-sm text-white disabled:opacity-50"
          >
            {isSubmitting ? "Completing…" : "Complete job"}
          </button>
        </div>
      </form>
    </div>
  );
}
