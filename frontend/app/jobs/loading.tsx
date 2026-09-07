import { JobsListSkeleton } from "@/presentation/views/jobs/components/molecules/jobs-list-skeleton.component";

export default function Loading() {
  return (
    <main className="mx-auto max-w-4xl p-6">
      <JobsListSkeleton />
    </main>
  );
}
