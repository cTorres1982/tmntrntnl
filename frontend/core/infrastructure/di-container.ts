import "server-only";
import { GetJobsUseCase } from "@/core/application/use-cases/get-jobs.use-case";
import { HttpJobRepository } from "./http-job-repository";

/**
 * Deliberately a small hand-rolled container (plain factory object), not a
 * decorator/reflect-metadata DI library — those complicate Server Components,
 * and a page needs exactly one use case, not a full IoC container. This is
 * what app/jobs/page.tsx means by "the DI container" in AC.md 2.1.1.
 */
function createContainer() {
  const jobRepository = new HttpJobRepository();

  return {
    getJobsUseCase: new GetJobsUseCase(jobRepository),
  };
}

export const container = createContainer();
