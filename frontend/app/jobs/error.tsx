"use client";

interface JobsErrorProps {
  error: Error & { digest?: string };
  reset: () => void;
}

/** Next.js requires error.tsx to be a Client Component — it renders in place of a Server Component that threw. */
export default function JobsError({ error, reset }: JobsErrorProps) {
  return (
    <main className="mx-auto max-w-4xl p-6">
      <div className="rounded-md border border-red-200 bg-red-50 p-4 text-sm text-red-700">
        <p>Something went wrong loading jobs: {error.message}</p>
        <button type="button" onClick={reset} className="mt-2 underline" data-testid="jobs-error-retry">
          Retry
        </button>
      </div>
    </main>
  );
}
