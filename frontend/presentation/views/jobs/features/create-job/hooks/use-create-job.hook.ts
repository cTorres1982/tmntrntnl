"use client";

import { useCallback, useReducer } from "react";
import { createJobAction } from "@/app/jobs/actions";
import { createJobFormReducer } from "../create-job-form.reducer";
import { INITIAL_CREATE_JOB_FORM_STATE } from "../create-job-form-state.type";
import type { CreateJobFormFields } from "../create-job-form-fields.type";

export function useCreateJob(onCreated: () => void) {
  const [state, dispatch] = useReducer(createJobFormReducer, INITIAL_CREATE_JOB_FORM_STATE);

  const onFieldChange = useCallback((field: keyof CreateJobFormFields, value: string) => {
    dispatch({ type: "field-changed", field, value });
  }, []);

  const onSubmit = useCallback(
    async (event: React.FormEvent<HTMLFormElement>) => {
      event.preventDefault();
      dispatch({ type: "submit-started" });

      const { fields } = state;
      const result = await createJobAction({
        title: fields.title,
        description: fields.description,
        street: fields.street,
        city: fields.city,
        state: fields.state,
        zipCode: fields.zipCode,
        latitude: Number(fields.latitude),
        longitude: Number(fields.longitude),
        customerId: fields.customerId,
        notes: fields.notes || undefined,
        scheduledDate: fields.scheduledDate || undefined,
        assigneeId: fields.assigneeId || undefined,
      });

      if (result.success) {
        dispatch({ type: "submit-succeeded" });
        onCreated();
      } else {
        dispatch({ type: "submit-failed", error: result.error ?? "Something went wrong." });
      }
    },
    [state, onCreated],
  );

  return {
    fields: state.fields,
    isSubmitting: state.isSubmitting,
    error: state.error,
    onFieldChange,
    onSubmit,
  };
}
