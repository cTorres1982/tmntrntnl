import "server-only";

/**
 * Server-only: the backend base URL is an internal detail that must never
 * ship to the browser bundle. `X-Organization-Id` stands in for a real
 * authenticated tenant claim — see backend/README.md and
 * TenantContextMiddleware for why (documented simplification, not
 * production auth).
 */
export const apiConfig = {
  baseUrl: process.env.BACKEND_API_URL ?? "http://localhost:5080",
  devOrganizationId: process.env.DEV_ORGANIZATION_ID ?? "11111111-1111-1111-1111-111111111111",
} as const;
