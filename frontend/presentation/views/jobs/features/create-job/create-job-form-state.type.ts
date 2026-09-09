import type { CreateJobFormErrors } from "./create-job-form-errors.type";
import type { CreateJobFormFields } from "./create-job-form-fields.type";

export interface CreateJobFormState {
  fields: CreateJobFormFields;
  fieldErrors: CreateJobFormErrors;
  isSubmitting: boolean;
  error: string | null;
}

export const INITIAL_CREATE_JOB_FORM_STATE: CreateJobFormState = {
  fields: {
    title: "",
    description: "",
    street: "",
    city: "",
    state: "",
    zipCode: "",
    latitude: "",
    longitude: "",
    customerId: "",
    notes: "",
    scheduledDate: "",
    assigneeId: "",
  },
  fieldErrors: {},
  isSubmitting: false,
  error: null,
};
