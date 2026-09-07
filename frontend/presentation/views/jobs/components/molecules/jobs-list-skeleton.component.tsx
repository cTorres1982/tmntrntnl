/** Shared by the Suspense fallback in page.tsx and app/jobs/loading.tsx. */
export function JobsListSkeleton() {
  const rows = Array.from({ length: 6 }, (_, index) => index);

  return (
    <div className="animate-pulse space-y-2" role="status" aria-label="Loading jobs" data-testid="jobs-list-skeleton">
      {rows.map((row) => (
        <div key={row} className="h-12 rounded-md bg-gray-200" />
      ))}
    </div>
  );
}
