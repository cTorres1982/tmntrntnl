import type { CreateJobFormFields } from "./create-job-form-fields.type";

export type CreateJobFormErrors = Partial<Record<keyof CreateJobFormFields, string>>;
