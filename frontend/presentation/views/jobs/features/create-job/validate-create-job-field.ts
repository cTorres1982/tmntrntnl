import type { CreateJobFormFields } from "./create-job-form-fields.type";

const LATITUDE_RANGE = { min: -90, max: 90 } as const;
const LONGITUDE_RANGE = { min: -180, max: 180 } as const;

// Matches any RFC 4122 variant/version — the backend only cares that
// System.Text.Json can parse the string into a Guid, not which version it is.
const GUID_PATTERN = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

function validateCoordinate(value: string, range: { min: number; max: number }, label: string): string | undefined {
  if (value.trim() === "") {
    return `${label} is required.`;
  }

  const parsed = Number(value);
  if (Number.isNaN(parsed)) {
    return `${label} must be a number.`;
  }

  if (parsed < range.min || parsed > range.max) {
    return `${label} must be between ${range.min} and ${range.max}.`;
  }

  return undefined;
}

/**
 * A malformed GUID string sent as JSON isn't something FluentValidation ever
 * sees — ASP.NET Core's model binding fails to deserialize it into a Guid
 * before the request even reaches CreateJobCommandValidator, producing a
 * generic 400 with none of our usual error shape. Catching the format here
 * avoids that entirely.
 */
function validateGuid(value: string, label: string, required: boolean): string | undefined {
  if (value.trim() === "") {
    return required ? `${label} is required.` : undefined;
  }

  if (!GUID_PATTERN.test(value.trim())) {
    return `${label} must be a valid GUID (e.g. 11111111-1111-1111-1111-111111111111).`;
  }

  return undefined;
}

/**
 * Mirrors the backend's own CreateJobCommandValidator range checks
 * (Latitude/Longitude InclusiveBetween — and Address.Create's identical
 * domain-layer check) so the frontend rejects the same values the API would,
 * before the round trip rather than after a 400 comes back.
 */
export function validateCreateJobField(field: keyof CreateJobFormFields, value: string): string | undefined {
  switch (field) {
    case "latitude":
      return validateCoordinate(value, LATITUDE_RANGE, "Latitude");
    case "longitude":
      return validateCoordinate(value, LONGITUDE_RANGE, "Longitude");
    case "customerId":
      return validateGuid(value, "Customer ID", true);
    case "assigneeId":
      return validateGuid(value, "Assignee ID", false);
    default:
      return undefined;
  }
}
