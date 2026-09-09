import type { CreateJobFormFields } from "./create-job-form-fields.type";

const LATITUDE_RANGE = { min: -90, max: 90 } as const;
const LONGITUDE_RANGE = { min: -180, max: 180 } as const;

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
    default:
      return undefined;
  }
}
