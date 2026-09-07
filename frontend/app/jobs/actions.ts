"use server";

import { revalidatePath } from "next/cache";
import { apiConfig } from "@/core/infrastructure/api-config";
import type { ActionResult } from "./action-result.type";
import type { CreateJobInput } from "./create-job-input.type";

/**
 * Server Actions handle mutations only (AC.md 2.1.7) — reads go through
 * GetJobsUseCase (page.tsx) or the /api/jobs route handler (client-side
 * React Query), never through a Server Action.
 */
export async function createJobAction(input: CreateJobInput): Promise<ActionResult> {
  const response = await fetch(`${apiConfig.baseUrl}/jobs`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Organization-Id": apiConfig.devOrganizationId,
    },
    body: JSON.stringify(input),
  });

  if (!response.ok) {
    const problem = (await response.json().catch(() => null)) as { detail?: string } | null;
    return { success: false, error: problem?.detail ?? `Failed to create job (${response.status}).` };
  }

  revalidatePath("/jobs");
  return { success: true };
}

export async function completeJobAction(jobId: string, signatureUrl: string): Promise<ActionResult> {
  const response = await fetch(`${apiConfig.baseUrl}/jobs/${jobId}/complete`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Organization-Id": apiConfig.devOrganizationId,
    },
    body: JSON.stringify({ signatureUrl }),
  });

  if (!response.ok) {
    const problem = (await response.json().catch(() => null)) as { detail?: string } | null;
    return { success: false, error: problem?.detail ?? `Failed to complete job (${response.status}).` };
  }

  revalidatePath("/jobs");
  return { success: true };
}
