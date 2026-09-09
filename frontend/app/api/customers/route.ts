import { NextResponse } from "next/server";
import { apiConfig } from "@/core/infrastructure/api-config";

/** Same BFF pattern as /api/jobs — proxies the customer picker's lookup so the browser never needs the backend URL or tenant header. */
export async function GET(): Promise<NextResponse> {
  const response = await fetch(`${apiConfig.baseUrl}/customers`, {
    headers: { "X-Organization-Id": apiConfig.devOrganizationId },
    cache: "no-store",
  });

  const body: unknown = await response.json().catch(() => null);
  return NextResponse.json(body, { status: response.status });
}
