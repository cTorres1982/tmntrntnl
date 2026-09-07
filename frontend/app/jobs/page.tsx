import "server-only";
import { Suspense } from "react";
import { JobsListSkeleton } from "@/presentation/views/jobs/components/molecules/jobs-list-skeleton.component";
import { JobsListLoader } from "./jobs-list-loader";

export default function JobsPage() {
  return (
    <main className="mx-auto max-w-4xl p-6">
      <Suspense fallback={<JobsListSkeleton />}>
        <JobsListLoader />
      </Suspense>
    </main>
  );
}
