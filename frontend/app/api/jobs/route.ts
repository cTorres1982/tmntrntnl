import { NextRequest, NextResponse } from "next/server";
import { apiConfig } from "@/core/infrastructure/api-config";

/**
 * Thin BFF proxy for the client-side filter-jobs feature (AC.md 2.1.7
 * explicitly allows "client-side SWR/React Query" for reads). Exists so the
 * browser never needs to know the backend's URL or send the tenant header
 * itself — same reasoning as HttpJobRepository, just reachable from client
 * components instead of only from the Server Component.
 */
export async function GET(request: NextRequest): Promise<NextResponse> {
  const upstreamUrl = `${apiConfig.baseUrl}/jobs?${request.nextUrl.searchParams.toString()}`;

  const response = await fetch(upstreamUrl, {
    headers: { "X-Organization-Id": apiConfig.devOrganizationId },
    cache: "no-store",
  });

  const body: unknown = await response.json().catch(() => null);
  return NextResponse.json(body, { status: response.status });
}
