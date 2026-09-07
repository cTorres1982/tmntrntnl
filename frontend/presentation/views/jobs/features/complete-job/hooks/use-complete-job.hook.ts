"use client";

import { useCallback, useState } from "react";
import { completeJobAction } from "@/app/jobs/actions";
import { useJobsStoreActions } from "../../../store/jobs-store.selectors";

/**
 * Optimistic update with rollback (AC.md 2.2.4): the job flips to "Completed"
 * in the store immediately, before the network call resolves, so the UI feels
 * instant. If the Server Action fails, the exact previous row
 * (applyOptimisticStatus returns it) is written back — no refetch needed to
 * recover.
 */
export function useCompleteJob(jobId: string, onCompleted: () => void) {
  const { applyOptimisticStatus, rollbackJob } = useJobsStoreActions();
  const [signatureUrl, setSignatureUrl] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const onSubmit = useCallback(
    async (event: React.FormEvent<HTMLFormElement>) => {
      event.preventDefault();
      setIsSubmitting(true);
      setError(null);

      const previous = applyOptimisticStatus(jobId, "Completed");

      const result = await completeJobAction(jobId, signatureUrl);

      setIsSubmitting(false);

      if (result.success) {
        onCompleted();
        return;
      }

      if (previous) {
        rollbackJob(jobId, previous);
      }
      setError(result.error ?? "Something went wrong.");
    },
    [jobId, signatureUrl, applyOptimisticStatus, rollbackJob, onCompleted],
  );

  return { signatureUrl, setSignatureUrl, isSubmitting, error, onSubmit };
}
