import type { CreateJobFormErrors } from "./create-job-form-errors.type";
import type { CreateJobFormFields } from "./create-job-form-fields.type";
import { validateCreateJobField } from "./validate-create-job-field";

export function validateCreateJobForm(fields: CreateJobFormFields): CreateJobFormErrors {
  const errors: CreateJobFormErrors = {};

  (Object.keys(fields) as (keyof CreateJobFormFields)[]).forEach((field) => {
    const error = validateCreateJobField(field, fields[field]);
    if (error) {
      errors[field] = error;
    }
  });

  return errors;
}
