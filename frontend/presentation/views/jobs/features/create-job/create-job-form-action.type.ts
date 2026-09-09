import type { CreateJobFormErrors } from "./create-job-form-errors.type";
import type { CreateJobFormFields } from "./create-job-form-fields.type";

export type CreateJobFormAction =
  | { type: "field-changed"; field: keyof CreateJobFormFields; value: string }
  | { type: "validation-failed"; errors: CreateJobFormErrors }
  | { type: "submit-started" }
  | { type: "submit-succeeded" }
  | { type: "submit-failed"; error: string }
  | { type: "reset" };
