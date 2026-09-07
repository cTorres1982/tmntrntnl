import type { CreateJobFormAction } from "./create-job-form-action.type";
import { INITIAL_CREATE_JOB_FORM_STATE, type CreateJobFormState } from "./create-job-form-state.type";

/**
 * useReducer (AC.md 2.3.3) rather than several useState calls: the form's
 * fields, submitting flag, and error all change together as one coherent
 * transition (e.g. "submit-started" clears the previous error AND sets
 * isSubmitting in the same update) — exactly the "multiple related fields
 * that change together" useReducer is for.
 */
export function createJobFormReducer(state: CreateJobFormState, action: CreateJobFormAction): CreateJobFormState {
  switch (action.type) {
    case "field-changed":
      return { ...state, fields: { ...state.fields, [action.field]: action.value } };
    case "submit-started":
      return { ...state, isSubmitting: true, error: null };
    case "submit-succeeded":
      return INITIAL_CREATE_JOB_FORM_STATE;
    case "submit-failed":
      return { ...state, isSubmitting: false, error: action.error };
    case "reset":
      return INITIAL_CREATE_JOB_FORM_STATE;
    default:
      return state;
  }
}
